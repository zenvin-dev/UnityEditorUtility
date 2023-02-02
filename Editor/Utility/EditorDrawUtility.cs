using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	public static class EditorDrawUtility {

		public enum Side {
			Left,
			Right,
			Top,
			Bottom
		}

		public enum LineStyle {
			Direct,
			Sharp,
			Bezier,
		}

		private static readonly Quaternion arrowAngleL = Quaternion.Euler (0f, 0f, 25f);
		private static readonly Quaternion arrowAngleR = Quaternion.Euler (0f, 0f, -25f);



		public static void DrawGrid (Rect position, Vector2 offset, float spacing, float thickness, Color color) {

			int countH = Mathf.CeilToInt (position.width / spacing) + 1;
			int countV = Mathf.CeilToInt (position.height / spacing) + 1;
			Color col = Handles.color;

			offset = new Vector2 (offset.x % spacing, offset.y % spacing);

			GUILayout.BeginArea (position);
			Handles.color = color;

			for (int i = 0; i < countH; i++) {
				float pos = offset.x + (spacing * i);
				Rect rect = new Rect (pos, 0, thickness, position.height);
				EditorGUI.DrawRect (rect, color);
				//Handles.DrawLine (rect.position, rect.position + Vector2.up * rect.height, thickness);
			}

			for (int i = 0; i < countV; i++) {
				float pos = offset.y + (spacing * i);
				Rect rect = new Rect (0, pos, position.width, thickness);
				EditorGUI.DrawRect (rect, color);
				//Handles.DrawLine (rect.position, rect.position + Vector2.right * rect.width, thickness);
			}

			Handles.color = col;
			GUILayout.EndArea ();
		}

		public static void DrawSeparator (Rect position, Side side, float thickness, Color color, bool inside) {
			if (Mathf.Approximately (thickness, 0f)) {
				return;
			}
			if (thickness < 0f) {
				inside ^= true;
				thickness *= -1f;
			}

			Rect? rect = null;
			Vector2 pos = position.position;

			Vector2 sizeHor = new Vector2 (position.width, thickness);
			Vector2 sizeVert = new Vector2 (thickness, position.height);

			switch (side) {
				case Side.Left:
					if (inside) {
						pos.x -= thickness;
					}
					rect = new Rect (pos, sizeVert);
					break;
				case Side.Right:
					pos.x += position.width;
					if (!inside) {
						pos.x -= thickness;
					}
					rect = new Rect (pos, sizeVert);
					break;
				case Side.Top:
					if (!inside) {
						pos.y -= thickness;
					}
					rect = new Rect (pos, sizeHor);
					break;
				case Side.Bottom:
					pos.y += position.height;
					if (inside) {
						pos.y -= thickness;
					}
					rect = new Rect (pos, sizeHor);
					break;
			}
			EditorGUI.DrawRect (rect.Value, color);
		}

		public static OrientedPoint DrawConnection (LineStyle style, Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, float thickness, Color color) {
			Color col = Handles.color;
			Handles.color = color;

			switch (style) {
				case LineStyle.Direct:
					Handles.DrawLine (start, end, thickness);
					break;
				case LineStyle.Sharp:
					Handles.DrawLine (start, start += startTangent, thickness);
					Handles.DrawLine (end, end += endTangent, thickness);
					Handles.DrawLine (start, end, thickness);
					break;
				case LineStyle.Bezier:
					Handles.DrawBezier (start, end, startTangent, endTangent, color, null, thickness);
					break;
			}

			Handles.color = color;
			return GetPointFromEnds (start, end);
		}

		internal static void DrawConnectionArrow (OrientedPoint ctr, float size, Color color) {
			size *= 0.5f;

			Vector3 leftFin = ctr.Position + (Vector2)(arrowAngleL * ctr.Orientation * Vector2.down) * size;
			Vector3 rightFin = ctr.Position + (Vector2)(arrowAngleR * ctr.Orientation * Vector2.down) * size;
			Vector3 tip = ctr.Position + (Vector2)(ctr.Orientation * Vector2.up) * size;

			var col = Handles.color;
			Handles.color = color;
			Handles.DrawAAConvexPolygon (ctr.Position, leftFin, tip, rightFin);
			Handles.color = col;
		}

		private static OrientedPoint GetPointFromEnds (Vector2 start, Vector2 end) {
			Vector2 dir = end - start;
			float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			return new OrientedPoint (start + dir * 0.5f, Quaternion.Euler (0f, 0f, angle));
		}

	}

	public struct OrientedPoint {
		public readonly Vector2 Position;
		public readonly Quaternion Orientation;

		public OrientedPoint (Vector2 position, Quaternion orientation) {
			Position = position;
			Orientation = orientation;
		}
	}
}