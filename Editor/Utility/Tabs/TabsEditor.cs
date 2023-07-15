using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.Tabs {
	public class TabsEditor {

		private readonly ITabsEditorCallbacks callbacks;

		private string[] tabNames;
		private int selectedTab;

		public int SelectedTab {
			get {
				return selectedTab;
			}
			set {
				if (tabNames == null || tabNames.Length == 0) {
					selectedTab = -1;
					return;
				}
				selectedTab = Mathf.Clamp (value, 0, tabNames.Length - 1);
			}
		}
		public Color SelectedTabTint { get; set; } = Color.gray;


		public TabsEditor (ITabsEditorCallbacks callbacks) {
			this.callbacks = callbacks;
			tabNames = callbacks?.GetTabNames (this);
		}


		public void Draw (Rect position) {
			if (callbacks == null) {
				return;
			}

			position.position = Vector2.zero;
			var bodyRect = position.Inset (RectTransform.Edge.Top, EditorGUIUtility.singleLineHeight, out Rect barRect);
			DrawTabHeader (barRect);
			DrawTabBody (bodyRect);
		}

		public void DrawTabHeader (Rect position) {
			if (callbacks == null) {
				return;
			}
			if (tabNames == null || tabNames.Length == 0) {
				return;
			}
			GUILayout.BeginArea (position);
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < tabNames.Length; i++) {
				var tint = i == SelectedTab ? SelectedTabTint : Color.white;
				var style = i == 0 ? EditorStyles.miniButtonLeft : (i == tabNames.Length - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid);

				GUI.backgroundColor = tint;
				if (GUILayout.Button (tabNames[i], style, GUILayout.ExpandWidth (true))) {
					SelectedTab = i;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
		}

		public void DrawTabBody (Rect position) {
			if (callbacks == null) {
				return;
			}
			callbacks.OnDrawTab (this, position, SelectedTab);
		}

	}
}