using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.ListTable {
	public class ListTableEditor {

		private readonly IListTableEditorCallbacks callbacks;

		private ColumnData[] columns;
		private float[] columnWidths;
		private float[] layoutColumnWidths;
		private Vector2 scroll;
		private float lastWidth;
		private GUIStyle rowbaseStyle;

		public float HeaderHeight { get; set; } = EditorGUIUtility.singleLineHeight;
		public float RowHeight { get; set; } = EditorGUIUtility.singleLineHeight;

		public Color RowBackgroundColor { get; set; } = Color.clear;
		public Color RowBackgroundColorAlt { get; set; } = UnityColors.SceneBackgroundColor;
		public Color RowBackgroundColorSelected { get; set; } = UnityColors.SelectionActiveColor;
		public Color SeparatorColor { get; set; } = UnityColors.SeparatorColor;

		private GUIStyle RowBaseStyle {
			get {
				if (rowbaseStyle == null) {
					rowbaseStyle = new GUIStyle (EditorStyles.label) {
						margin = new RectOffset (),
						padding = new RectOffset (),
					};
				}
				return rowbaseStyle;
			}
		}


		public ListTableEditor (IListTableEditorCallbacks callbacks, params ColumnData[] columns) {
			this.callbacks = callbacks;
			SetColumnHeaders (columns);
		}


		public void Draw (Rect position) {
			if (callbacks == null || columns == null || columns.Length == 0 || position.width <= 0f) {
				return;
			}

			UpdateColumnWidths (position.width - 13f);

			position = position.Inset (RectTransform.Edge.Top, HeaderHeight, out Rect headerRect);
			GUILayout.BeginArea (position);
			DrawRows ();
			GUILayout.EndArea ();

			DrawColumnHeaders (headerRect);
		}

		public void SetColumnHeaders (params ColumnData[] columns) {
			this.columns = columns;
		}


		private void UpdateColumnWidths (float totalWidth) {
			if (columns == null) {
				columnWidths = null;
				return;
			}

			if (columnWidths == null || columns.Length != columnWidths.Length) {
				columnWidths = new float[columns.Length];
			}
			if (layoutColumnWidths == null || columns.Length != layoutColumnWidths.Length) {
				layoutColumnWidths = new float[columns.Length];
			}

			float remaining = totalWidth;
			int numDynamic = 0;
			for (int i = 0; i < columns.Length; i++) {
				var col = columns[i];
				if (col.Width.HasValue) {
					columnWidths[i] = col.Width.Value;
					remaining -= col.Width.Value;
				} else {
					numDynamic++;
				}
			}

			if (numDynamic < columns.Length) {
				float fraction = remaining / numDynamic;
				for (int i = 0; i < columns.Length; i++) {
					var col = columns[i];
					if (!col.Width.HasValue) {
						columnWidths[i] = fraction;
					}
				}
			}
		}

		private void DrawColumnHeaders (Rect headerRect) {
			float offset = 0f;

			for (int i = 0; i < columns.Length; i++) {
				var cell = columns[i];
				var cont = cell.Header ?? GUIContent.none;

				var rect = new Rect (headerRect);
				rect.x += offset;
				rect.width = layoutColumnWidths[i];
				offset += rect.width;

				EditorGUI.LabelField (rect, cont, EditorStyles.boldLabel);
				EditorDrawUtility.DrawSeparator (rect, EditorDrawUtility.Side.Right, 1f, SeparatorColor, true);
				EditorDrawUtility.DrawSeparator (rect, EditorDrawUtility.Side.Bottom, 1f, SeparatorColor, true);
			}
			
		}

		private void DrawRows () {
			scroll = EditorGUILayout.BeginScrollView (scroll);

			var rowCount = callbacks.GetRowCount (this);
			for (int i = 0; i < rowCount; i++) {
				var rowData = callbacks.GetRowContent (this, i);
				var content = rowData.Content;
				if (content == null || content.Length == 0) {
					continue;
				}

				var columnCount = Mathf.Min (columns.Length, content.Length);
				if (columnCount <= 0) {
					continue;
				}

				DrawRow (i, columnCount, rowData);
			}

			EditorGUILayout.EndScrollView ();
		}

		private void DrawRow (int rowIndex, int columnCount, RowData rowData) {
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < columnCount; i++) {
				var tint = rowData.Selected ? RowBackgroundColorSelected : (rowIndex % 2 == 0 ? RowBackgroundColorAlt : RowBackgroundColor);
				var cell = rowData.Content[i];
				var cont = cell.Content ?? GUIContent.none;
				var style = cell.Style ?? EditorStyles.label;
				var rect = GUILayoutUtility.GetRect (columnWidths[i], RowHeight);

				layoutColumnWidths[i] = rect.width;

				EditorGUI.DrawRect (rect, tint);
				if (cell.Selectable) {
					EditorGUI.SelectableLabel (rect, cont.text, style);
				} else {
					EditorDrawUtility.Button (rect, cont, style, out bool btn, out bool ctx);
					if (btn || ctx) {
						callbacks.OnClickCell (this, rowIndex, i, ctx);
					}
				}
				EditorDrawUtility.DrawSeparator (rect, EditorDrawUtility.Side.Right, 1f, SeparatorColor, true);
			}
			GUILayout.EndHorizontal ();
		}
	}
}