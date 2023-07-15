using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil.Sidebar;
using static Zenvin.EditorUtil.MathUtility;

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

		public enum ArrowStyle {
			Triangle,
			Indented,
		}

		private static readonly Quaternion arrowAngleSharpL = Quaternion.Euler (0f, 0f, 45f);
		private static readonly Quaternion arrowAngleSharpR = Quaternion.Euler (0f, 0f, -45f);
		private static readonly Quaternion arrowAngleL = Quaternion.Euler (0f, 0f, 60f);
		private static readonly Quaternion arrowAngleR = Quaternion.Euler (0f, 0f, -60f);

		private static GUIStyle tabButtonStyle;
		private static GUIStyle TabButtonStyle {
			get {
				if (tabButtonStyle == null) {
					tabButtonStyle = new GUIStyle (EditorStyles.label);
					tabButtonStyle.alignment = TextAnchor.MiddleCenter;
				}
				return tabButtonStyle;
			}
		}

		private static GUIStyle layoutButtonStyle;
		private static GUIStyle LayoutButtonStyle {
			get {
				if (layoutButtonStyle == null) {
					layoutButtonStyle = new GUIStyle (EditorStyles.label) {
						margin = new RectOffset (),
						padding = new RectOffset (),
						alignment = TextAnchor.MiddleLeft,
						richText = true,
					};
				}
				return layoutButtonStyle;
			}
		}


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
					if (!inside) {
						pos.x -= thickness;
					}
					rect = new Rect (pos, sizeVert);
					break;
				case Side.Right:
					pos.x += position.width;
					if (inside) {
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
			OrientedPoint point = new OrientedPoint ();
			Color col = Handles.color;
			Handles.color = color;

			switch (style) {
				case LineStyle.Direct:
					Handles.DrawAAPolyLine (thickness, start, end);
					break;
				case LineStyle.Sharp:
					Handles.DrawLine (start, start + startTangent, thickness);
					Handles.DrawSolidDisc (start + startTangent, Vector3.forward, thickness * 0.5f);
					Handles.DrawSolidDisc (end + endTangent, Vector3.forward, thickness * 0.5f);
					break;
				case LineStyle.Bezier:
					Handles.DrawAAPolyLine (thickness, GetBezierPoints (start, start + startTangent, end, end + endTangent, 20));
					point = GetOrientedBezierPoint (start, start + startTangent, end, end + endTangent, 0.5f);
					break;
			}

			Handles.color = col;
			return point;
		}

		public static void DrawConnectionArrow (OrientedPoint ctr, ArrowStyle style, float size, Color color) {
			size *= 0.5f;

			Vector3 leftFin;
			Vector3 rightFin;
			Vector3 tip;

			var col = Handles.color;
			Handles.color = color;
			switch (style) {
				case ArrowStyle.Indented:
					leftFin = ctr.Position + (Vector2)(arrowAngleSharpL * ctr.Orientation * Vector2.left) * size;
					rightFin = ctr.Position + (Vector2)(arrowAngleSharpR * ctr.Orientation * Vector2.left) * size;
					tip = ctr.Position + (Vector2)(ctr.Orientation * Vector2.right) * size;

					Handles.DrawAAConvexPolygon (ctr.Position, leftFin, tip, rightFin);
					break;
				case ArrowStyle.Triangle:
					Quaternion rotOffset = Quaternion.Euler (0f, -90f, 0f);
					leftFin = ctr.Position + (Vector2)(arrowAngleL * ctr.Orientation * rotOffset * Vector2.left) * size;
					rightFin = ctr.Position + (Vector2)(arrowAngleR * ctr.Orientation * rotOffset * Vector2.left) * size;
					tip = ctr.Position + (Vector2)(ctr.Orientation * rotOffset * Vector2.right) * size;

					Handles.DrawAAConvexPolygon (leftFin, tip, rightFin);
					break;
			}
			Handles.color = col;
		}

		public static void DrawBeveledRect (Rect rect, float bevel, int bevelResolution, Color color) {
			bevel = Mathf.Abs (bevel);
			bevelResolution = Mathf.Abs (bevelResolution);

			var points = GetPointsOnBeveledRect (rect, bevel, bevelResolution);
			var col = Handles.color;

			Handles.color = color;
			Handles.DrawAAConvexPolygon (points);
			Handles.color = col;
		}

		public static void Button (Rect rect, GUIContent content, GUIStyle style, out bool button, out bool context, Event e = null) {
			if (e == null) {
				e = Event.current;
			}
			button = GUI.Button (rect, content, style);
			context = button && (e.type == EventType.Used && (e.button == 1 || e.keyCode == KeyCode.Mouse1));
		}

		public static void Button (Rect rect, GUIContent content, bool active, out bool button, out bool context, Event e = null) {
			if (e == null) {
				e = Event.current;
			}

			var color = active ? UnityColors.SelectionActiveColor : (rect.Contains (e.mousePosition) ? UnityColors.ForegroundColor : Color.clear);
			EditorGUI.DrawRect (rect, color);

			button = GUI.Button (rect, content, LayoutButtonStyle);
			context = button && (e.type == EventType.Used && (e.button == 1 || e.keyCode == KeyCode.Mouse1));
		}

		public static bool Button (Rect rect, GUIContent content, bool active, Event e = null) {
			if (e == null) {
				e = Event.current;
			}

			var color = active ? UnityColors.SelectionActiveColor : (rect.Contains (e.mousePosition) ? UnityColors.ForegroundColor : Color.clear);
			EditorGUI.DrawRect (rect, color);
			return GUI.Button (rect, content, EditorStyles.label);
		}

		public static void LayoutButton (GUIContent content, bool active, out bool button, out bool context, Event e = null, params GUILayoutOption[] options) {
			var rect = GUILayoutUtility.GetRect (content, LayoutButtonStyle, options);
			Button (rect, content, active, out button, out context, e);
		}

		public static bool LayoutButton (GUIContent content, bool active, Event e = null, params GUILayoutOption[] options) {
			var rect = GUILayoutUtility.GetRect (content, LayoutButtonStyle, options);
			return Button (rect, content, active, e);
		}

		public static void DrawLayoutTabs (ref int selectedTab, Event currentEvent, float height, bool drawSeparators, params ButtonBar.SidebarButton[] tabs) {
			if (height <= 0 || tabs == null || tabs.Length == 0 || currentEvent == null) {
				return;
			}

			Rect fullRect = EditorGUILayout.GetControlRect (GUILayout.Height (height));
			fullRect.width += 9f;
			float buttonWidth = fullRect.width / tabs.Length;

			for (int i = 0; i < tabs.Length; i++) {
				Rect rect = new Rect (buttonWidth * i, 0f, buttonWidth, height);
				Color col = selectedTab == i ? UnityColors.SelectionActiveColor : (rect.Contains (currentEvent.mousePosition) ? UnityColors.SelectionInactiveColor : Color.clear);

				EditorGUI.DrawRect (rect, col);
				if (GUI.Button (rect, tabs[i].Label, TabButtonStyle)) {
					selectedTab = i;
				}

				if (drawSeparators && i < tabs.Length - 1) {
					DrawSeparator (rect, Side.Right, 1f, UnityColors.SeparatorColor, true);
				}
			}
		}

		public static int DrawLayoutChips (string label, bool horizontal, params string[] chips) {
			if (chips == null || chips.Length == 0) {
				return -1;
			}

			int index = -1;
			if (horizontal) {
				GUILayout.BeginHorizontal ();
			}

			if (!string.IsNullOrWhiteSpace (label)) {
				GUILayout.Label (label, EditorStyles.boldLabel, GUILayout.ExpandWidth (false));
			}

			for (int i = 0; i < chips.Length; i++) {
				if (GUILayout.Button (chips[i], EditorStyles.miniButton, GUILayout.ExpandWidth (false))) {
					GUI.FocusControl (null);
					index = i;
				}
			}

			if (horizontal) {
				GUILayout.EndHorizontal ();
			}
			return index;
		}

		public static int DrawLayoutChips (bool horizontal, params ChipContent[] chips) {
			if (chips == null || chips.Length == 0) {
				return -1;
			}

			int index = -1;
			if (horizontal) {
				GUILayout.BeginHorizontal ();
			}

			for (int i = 0; i < chips.Length; i++) {
				var chip = chips[i];

				if (i < chips.Length - 1 && chip.Separate) {
					var separatorRect = GUILayoutUtility.GetRect (7f, EditorGUIUtility.singleLineHeight);
					separatorRect.width = 1f;
					separatorRect.x += 3f;
					EditorGUI.DrawRect (separatorRect, UnityColors.SceneMajorGridColor);
				}

				if (!string.IsNullOrWhiteSpace (chip.Prefix)) {
					GUILayout.Label (chip.Prefix, EditorStyles.boldLabel, GUILayout.ExpandWidth (false));
				}

				if (GUILayout.Button (new GUIContent(chip.Label, chip.Tooltip), EditorStyles.miniButton, GUILayout.ExpandWidth (false))) {
					GUI.FocusControl (null);
					index = i;
				}
			}

			if (horizontal) {
				GUILayout.EndHorizontal ();
			}
			return index;
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

	public readonly struct ChipContent {
		public readonly string Prefix;
		public readonly string Label;
		public readonly bool Separate;
		public readonly string Tooltip;


		public ChipContent (string label) : this (null, label, false, null) { }

		public ChipContent (string label, string tooltip) : this (null, label, false, tooltip) { }

		public ChipContent (string prefix, string label, string tooltip) : this (prefix, label, false, tooltip) { }

		public ChipContent (string prefix, string label, bool separate) : this(prefix, label, separate, null) { }

		public ChipContent (string prefix, string label, bool separate, string tooltip) {
			Prefix = prefix;
			Label = label;
			Separate = separate;
			Tooltip = tooltip;
		}
	}
}