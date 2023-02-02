using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.Sidebar {
	public class ButtonSidebar : ButtonBar {
		private float scrollOffset = 0f;
		private float lastHeight = -1f;

		public float TooltipWidth { get; set; } = 200f;
		public Texture2D FallbackIcon { get; set; }
		public int SelectedTab { get; private set; } = -1;
		public bool AllowDeselectTab { get; set; } = false;

		public int TotalButtonCount => tabs.Count + buttons.Count;


		public ButtonSidebar (ISidebarCallbacks callbacks) : base (callbacks) { }


		public void AddButton (string label, Texture2D icon) {
			if (string.IsNullOrWhiteSpace (label) && icon == null) {
				Debug.LogError ("Cannot add empty button to sidebar.");
				return;
			}
			SidebarButton btn = new SidebarButton (label, icon);
			buttons.Add (btn);
		}

		public void AddTab (string label, Texture2D icon) {
			if (string.IsNullOrWhiteSpace (label) && icon == null) {
				Debug.LogError ("Cannot add empty tab to sidebar.");
				return;
			}
			SidebarButton btn = new SidebarButton (label, icon);
			tabs.Add (btn);
		}


		public override bool Draw (Rect position) {
			if (position.width <= 0 || position.height <= 0) {
				Debug.LogError ("Cannot draw sidebar with a width or height equal of less than 0.");
				return false;
			}

			bool redraw = true;
			Event e = Event.current;

			GUILayout.BeginArea (position);

			Rect pos = new Rect (position);
			pos.y = -scrollOffset;
			pos.x = 0f;
			pos.height = GetRequiredHeight (position.width);

			GUILayout.BeginArea (pos);
			GUILayout.BeginVertical ();

			for (int i = 0; i < tabs.Count; i++) {
				if (i > 0) {
					GUILayout.Space (ButtonSpacing);
				}
				DrawTab (position, i, e);
			}

			DrawSeparator (position);

			for (int i = 0; i < buttons.Count; i++) {
				DrawButton (position, i, e);
				GUILayout.Space (ButtonSpacing);
			}

			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			GUILayout.EndArea ();


			float heightDiff = lastHeight - position.height;
			if (heightDiff < 0) {
				scrollOffset += heightDiff;
				redraw = true;
			}

			if (position.Contains (e.mousePosition)) {
				if (e.type == EventType.ScrollWheel && pos.height > position.height) {
					scrollOffset += e.delta.y * 3f;
					e.Use ();
					redraw = true;
				}
			}

			scrollOffset = Mathf.Clamp (scrollOffset, 0f, pos.height - position.width);
			lastHeight = position.height;
			if (redraw) {
				callbacks.QueueRepaint ();
			}
			return redraw;
		}

		public override void DrawTooltip (Rect sidebarPosition) {
			if (ActiveTooltip == null) {
				return;
			}

			GUIContent content = new GUIContent (ActiveTooltip.Label);

			Rect r = new Rect (ActiveTooltip.TargetPosition);
			r.x += sidebarPosition.width;
			r.width = TooltipWidth;
			EditorGUI.DrawRect (r, UnityColors.BackgroundColor);
			EditorGUI.LabelField (r, content, TooltipLabelStyle);

			ActiveTooltip = null;
		}

		public bool SetSelectedTab (int index) {
			index = Mathf.Clamp (index, -1, tabs.Count - 1);
			if (index == -1 && !AllowDeselectTab) {
				return false;
			}
			SelectedTab = index;
			return true;
		}


		private void DrawTab (Rect position, int i, Event e) {
			SidebarButton btn = tabs[i];
			GUIContent btnCont = new GUIContent (btn.Icon ?? FallbackIcon);
			GUIStyle stl = SelectedTab == i ? ButtonStyleActive : ButtonStyle;

			Rect btnRect = GUILayoutUtility.GetRect (position.width, position.width);
			if (GUI.Button (btnRect, btnCont, stl)) {
				if (i != SelectedTab) {
					callbacks?.TabChanged (this, i, SelectedTab);
					SelectedTab = i;
				} else if (AllowDeselectTab) {
					callbacks?.TabChanged (this, -1, SelectedTab);
					SelectedTab = -1;
				}
			}

			if (btnRect.Contains (e.mousePosition)) {
				ActiveTooltip = new TooltipContent (btnRect, btn.Label);
			}
		}

		private void DrawButton (Rect position, int i, Event e) {
			SidebarButton btn = buttons[i];
			GUIContent btnCont = new GUIContent (btn.Icon ?? FallbackIcon);
			Rect btnRect = GUILayoutUtility.GetRect (position.width, position.width);

			EditorGUI.BeginDisabledGroup (callbacks != null && !callbacks.ButtonEnabled (this, i));
			if (GUI.Button (btnRect, btnCont, ButtonStyle)) {
				callbacks?.ButtonClicked (this, i);
			}
			EditorGUI.EndDisabledGroup ();

			if (btnRect.Contains (e.mousePosition)) {
				ActiveTooltip = new TooltipContent (btnRect, btn.Label);
			}
		}

		private void DrawSeparator (Rect position) {
			GUILayout.Space (SeparatorSpacing);
			Rect sepRect = GUILayoutUtility.GetRect (position.width, SeparatorHeight);
			sepRect.x += SeparatorMargin;
			sepRect.width -= SeparatorMargin * 2;
			EditorGUI.DrawRect (sepRect, UnityColors.SeparatorColor);
			GUILayout.Space (SeparatorSpacing);
		}


		private float GetRequiredHeight (float width) {
			float height = (width + ButtonSpacing) * buttons.Count;
			if (tabs.Count > 0) {
				height += SeparatorHeight + SeparatorSpacing * 2;
				height += (width + ButtonSpacing) * tabs.Count;
			}
			return height;
		}

	}
}