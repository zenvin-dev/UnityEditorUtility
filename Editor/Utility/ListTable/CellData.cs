using UnityEngine;

namespace Zenvin.EditorUtil.ListTable {
	public struct RowData {
		public bool Selected;
		public CellData[] Content;
	}

	public struct CellData {
		public GUIContent Content;
		public GUIStyle Style;
		public bool Selectable;

		public CellData (string text) : this () {
			Content = new GUIContent (text);
		}

		public CellData (string text, string tooltip) : this () {
			Content = new GUIContent (text, tooltip);
		}

		public CellData (string text, string tooltip, Texture icon) : this () {
			Content = new GUIContent (text, icon, tooltip);
		}

		public CellData (string text, Texture icon) : this () {
			Content = new GUIContent (text, icon);
		}
	}

	public struct ColumnData {
		public GUIContent Header;
		public float? Width;

		public ColumnData (string title) {
			Header = new GUIContent (title);
			Width = null;
		}

		public ColumnData (string title, float width) : this(title) {
			Width = width;
		}
	}
}