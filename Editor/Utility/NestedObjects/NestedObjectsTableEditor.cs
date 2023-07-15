using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil.ListTable;

namespace Zenvin.EditorUtil.NestedObjects {
	public sealed class NestedObjectsTableEditor<TParent, TChild> : NestedObjectsEditorBase<INestedObjectsTableCallbacks<TParent, TChild>, TParent, TChild>,
		IListTableEditorCallbacks
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		private readonly ListTableEditor itemTable;


		public NestedObjectsTableEditor (INestedObjectsTableCallbacks<TParent, TChild> callbacks) : base (callbacks) {
			if (callbacks == null) {
				return;
			}
			itemTable = new ListTableEditor (this, callbacks.GetColumns (this));
		}


		protected override void DrawList (Rect position) {
			itemTable?.Draw (position);
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

		RowData IListTableEditorCallbacks.GetRowContent (ListTableEditor source, int row) {
			var hasQuery = !string.IsNullOrEmpty (SearchQuery);
			var index = hasQuery ? GetIndex (row) : row;
			return new RowData () { Selected = Selected.Contains (index), Content = Callbacks.GetChildRowContent (this, index) };
		}

		int IListTableEditorCallbacks.GetRowCount (ListTableEditor source) {
			var hasQuery = !string.IsNullOrEmpty (SearchQuery);
			return hasQuery ? SearchResultCount : Callbacks.GetChildCount (this);
		}

		void IListTableEditorCallbacks.OnClickCell (ListTableEditor source, int row, int column, bool context) {
			var e = Event.current;
			GUI.FocusControl (null);

			if (context) {
				Menu = new GenericMenu ();
				Menu.AddItem (new GUIContent ("Delete selected"), false, OnDeleteSelected);
				Callbacks.OnContextClick (this, GetSelectedChildren (), Menu);
			} else {
				Selected.Select (row, e.shift, e.control || e.command);
			}
		}
	}
}