using System;
using UnityEngine;

namespace Zenvin.EditorUtil {
	public static class MathUtility {

		public static Vector3[] GetPointsOnBeveledRect (Rect rect, float bevel, int bevelResolution) {
			float doubleBevel = bevel * 2f;
			if (bevelResolution <= 3 || bevel <= 0.01f || rect.width < doubleBevel || rect.height < doubleBevel) {
				return new Vector3[] {
					new Vector3 (0f, 0f, 0f),
					new Vector3 (rect.width, 0f, 0f),
					new Vector3 (rect.width, rect.height),
					new Vector3 (0f, rect.height)
				};
			}

			Vector3[] points = new Vector3[8 + (bevelResolution - 2) * 4];
			int index = 0;

			// top - left
			points[index++] = rect.position + new Vector2 (bevel, 0f);
			// top - right
			points[index++] = rect.position + new Vector2 (rect.width - bevel, 0f);

			// top right quarter
			Vector3 topRightCenter = rect.position + new Vector2 (rect.width - bevel, bevel);
			Vector3[] topRightPoints = CalculatePointsOnCircle (bevelResolution, bevel, 270f, 90f, true);
			for (int i = 0; i < topRightPoints.Length; i++) {
				points[index++] = topRightCenter + topRightPoints[i];
			}

			// right - top
			points[index++] = rect.position + new Vector2 (rect.width, bevel);
			// right - bottom
			points[index++] = rect.position + new Vector2 (rect.width, rect.height - bevel);

			// bottom right quarter
			Vector3 btmRightCenter = rect.position + new Vector2 (rect.width - bevel, rect.height - bevel);
			Vector3[] btmRightPoints = CalculatePointsOnCircle (bevelResolution, bevel, 0f, 90f, true);
			for (int i = 0; i < btmRightPoints.Length; i++) {
				points[index++] = btmRightCenter + btmRightPoints[i];
			}

			// bottom - right
			points[index++] = rect.position + new Vector2 (rect.width - bevel, rect.height);
			// bottom - left
			points[index++] = rect.position + new Vector2 (bevel, rect.height);

			// bottom left quarter
			Vector3 btmLeftCenter = rect.position + new Vector2 (bevel, rect.height - bevel);
			Vector3[] btmLeftPoints = CalculatePointsOnCircle (bevelResolution, bevel, 90f, 90f, true);
			for (int i = 0; i < btmLeftPoints.Length; i++) {
				points[index++] = btmLeftCenter + btmLeftPoints[i];
			}

			// left - bottom
			points[index++] = rect.position + new Vector2 (0f, rect.height - bevel);
			// left - top
			points[index++] = rect.position + new Vector2 (0f, bevel);

			// bottom left quarter
			Vector3 topLeftCenter = rect.position + new Vector2 (bevel, bevel);
			Vector3[] topLeftPoints = CalculatePointsOnCircle (bevelResolution, bevel, 180f, 90f, true);
			for (int i = 0; i < topLeftPoints.Length; i++) {
				points[index++] = topLeftCenter + topLeftPoints[i];
			}

			return points;
		}

		public static Vector3 GetDirectionFromAngle (float angle) {
			angle *= Mathf.Deg2Rad;
			return new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0f).normalized;
		}

		public static OrientedPoint GetPointFromEnds (Vector2 start, Vector2 end) {
			Vector2 dir = end - start;
			float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			return new OrientedPoint (start + dir * 0.5f, Quaternion.Euler (0f, 0f, angle));
		}

		public static Vector3[] GetBezierPoints (Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent, int pointCount) {
			pointCount++;
			if (pointCount <= 2) {
				return Array.Empty<Vector3> ();
			}
			if (pointCount == 2) {
				return new Vector3[] { start, end };
			}

			Vector3[] points = new Vector3[pointCount];
			float step = 1f / (pointCount - 1);

			if (IsStraigtCurve (start, startTangent, end, endTangent)) {
				for (int i = 0; i < pointCount - 1; i++) {
					points[i] = Vector3.Lerp (start, end, step * i);
				}
				points[pointCount - 1] = end;
				return points;
			}

			for (int i = 0; i < pointCount - 1; i++) {
				points[i] = CalculateBezierPointCubic (start, startTangent, endTangent, end, step * i);
			}
			points[pointCount - 1] = end;
			return points;
		}

		public static Vector3 GetBezierPoint (Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent, float t) {
			return CalculateBezierPointCubic (start, startTangent, endTangent, end, t);
		}

		public static OrientedPoint GetOrientedBezierPoint (Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent, float t) {
			if (IsStraigtCurve (start, startTangent, end, endTangent)) {
				return new OrientedPoint (Vector3.Lerp (start, end, t), Quaternion.LookRotation (end - start, Vector3.forward));
			}

			const float offset = 0.05f;
			t = Mathf.Clamp01 (t);

			Vector3 point = CalculateBezierPointCubic (start, startTangent, endTangent, end, t);
			Vector3 direction;
			if (t < offset) {
				direction = startTangent.normalized;
			} else if (t > (1 - offset)) {
				direction = endTangent.normalized;
			} else {
				Vector3 p0 = CalculateBezierPointCubic (start, startTangent, endTangent, end, t - offset);
				Vector3 p1 = CalculateBezierPointCubic (start, startTangent, endTangent, end, t + offset);
				direction = (p1 - p0).normalized;
			}
			return new OrientedPoint (point, direction.sqrMagnitude == 0f ? Quaternion.identity : Quaternion.LookRotation (direction, Vector3.forward));
		}

		public static float GetClosestDistanceToCurve (Vector3 point, Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent, int pointCount, int iterations) {
			return GetClosestDistanceToCurveRecursive (point, 0f, 1f, start, startTangent, endTangent, end, pointCount, iterations);
		}

		public static bool IsStraigtCurve (Vector3 start, Vector3 startTangent, Vector3 end, Vector3 endTangent) {
			Vector3 dirStart = (start - startTangent).normalized;
			Vector3 dirEnd = (end - endTangent).normalized;
			Vector3 dir = (end - start).normalized;

			float angleTangent = Vector3.Angle (dirStart, dirEnd);
			float angleDir = Vector3.Angle (dirStart, dir);

			return (angleDir < 0.05f || angleDir > 179.05f) && (angleTangent < 0.05f || angleTangent > 179.05f);
		}


		private static float GetClosestDistanceToCurveRecursive (Vector3 point, float start, float end, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int pointCount, int iterations) {
			float step = (end - start) / pointCount;
			float best = 0f;
			float bestDist = float.PositiveInfinity;
			float currentDist;
			float t = start;

			while (t <= end) {
				Vector3 pt = CalculateBezierPointCubic (v0, v1, v2, v3, t);

				currentDist = (pt - point).sqrMagnitude;
				if (currentDist <= bestDist) {
					bestDist = currentDist;
					best = t;
				}

				t += step;
			}

			if (iterations <= 0) {
				return bestDist;
			}

			return GetClosestDistanceToCurveRecursive (point, Mathf.Max (best - step, 0f), Mathf.Min (best + step, 1f), v0, v1, v2, v3, pointCount, iterations - 1);
		}

		private static Vector3[] CalculatePointsOnCircle (int count, float radius, float startAngle, float angleSpan, bool excludeOuter) {
			if (excludeOuter) {
				count -= 2;
			}
			count = Mathf.Max (0, count);

			if (count == 0 || radius == 0f || angleSpan == 0f) {
				return null;
			}

			Vector3[] points = new Vector3[count];

			float step;
			if (excludeOuter) {
				step = angleSpan / (count + 1);
				startAngle += step;
			} else {
				step = angleSpan / (count);
			}

			for (int i = 0; i < count; i++) {
				points[i] = GetDirectionFromAngle (startAngle + step * i) * radius;
			}

			return points;
		}

		private static Vector3 CalculateBezierPointCubic (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float t) {
			float x = CalculateBezierValueCubic (v0.x, v1.x, v2.x, v3.x, t);
			float y = CalculateBezierValueCubic (v0.y, v1.y, v2.y, v3.y, t);
			float z = CalculateBezierValueCubic (v0.z, v1.z, v2.z, v3.z, t);
			return new Vector3 (x, y, z);
		}

		private static float CalculateBezierValueCubic (float p0, float p1, float p2, float p3, float t) {
			t = Mathf.Clamp01 (t);
			return (1 - t) * (1 - t) * (1 - t) * p0 + 3 * (1 - t) * (1 - t) * t * p1 + 3 * (1 - t) * t * t * p2 + t * t * t * p3;
		}

	}
}