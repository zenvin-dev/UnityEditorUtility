using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.NestedObjects {
	public sealed class NestedObjectsEditor<TParent, TChild> : NestedObjectsEditorBase<INestedObjectsCallbacks<TParent, TChild>, TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		private Vector2 listScroll;


		public NestedObjectsEditor (INestedObjectsCallbacks<TParent, TChild> callbacks) : base (callbacks) {

		}

		protected override void DrawToolbar (Rect position) {
			GUILayout.BeginArea (position);
			GUILayout.BeginHorizontal ();

			var add = EditorGUILayout.DropdownButton (AddItemContent, FocusType.Keyboard, EditorStyles.toolbarDropDown, GUILayout.Width (30f));
			var btnRect = GUILayoutUtility.GetLastRect ();
			if (add) {
				CreateAddItemMenu (btnRect);
			}

			var search = EditorGUILayout.DelayedTextField (SearchQuery, EditorStyles.toolbarSearchField);
			SetSearchQuery (search);

			if (SearchResultCount > 0) {
				EditorGUILayout.LabelField ($"{SearchResultCount} / {Callbacks.GetChildCount (this)}", GUILayout.ExpandWidth (false));
			}

			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
		}

		protected override void DrawList (Rect position) {
			Event e = Event.current;

			position = position.Inset (0f, 0f, EditorGUIUtility.standardVerticalSpacing, 0f);
			GUILayout.BeginArea (position);
			listScroll = GUILayout.BeginScrollView (listScroll);

			var hasQuery = !string.IsNullOrEmpty (SearchQuery);
			var count = hasQuery ? SearchResultCount : Callbacks.GetChildCount (this);
			for (int i = 0; i < count; i++) {
				var index = hasQuery ? GetIndex (i) : i;
				var content = Callbacks.GetChildContent (this, index);
				EditorDrawUtility.LayoutButton (content, Selected.Contains (index), out bool btn, out bool ctx, e);
				if (btn) {
					Selected.Select (index, e.shift, e.control || e.command);
				}
				if (ctx) {
					Menu = new GenericMenu ();
					Callbacks.OnContextClick (this, GetSelectedChildren (), Menu);
				}
			}
			GUILayout.EndArea ();
			GUILayout.EndArea ();
		}
	}
}