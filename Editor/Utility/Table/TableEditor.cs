using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zenvin.EditorUtil.Table {
	public class TableEditor {

		private readonly ITableEditorCallbacks callbacks;

		private const float ScrollbarSize = 13f;

		private Vector2 scrollPosition;
		private Vector2 cellSize;
		private Vector2 headerSize;

		public float RowHeaderWidth { get => headerSize.x; set => headerSize.x = Mathf.Max (value, 50f); }
		public float ColumnHeaderHeight { get => headerSize.y; set => headerSize.y = Mathf.Max (value, EditorGUIUtility.singleLineHeight * 2f); }
		public float ColumnWidth { get => cellSize.x; set => cellSize.x = Mathf.Max (value, 50f); }
		public float RowHeight { get => cellSize.y; set => cellSize.y = Mathf.Max (value, EditorGUIUtility.singleLineHeight); }

		public Vector2Int SelectedCell { get; set; }

		/// <summary> The normal color for each header cell. </summary>
		public Color HeaderColorDefault { get; set; } = UnityColors.SceneBackgroundColor;
		/// <summary> The highlighted color for each header cell. Used when a cell in the corresponding row/column is selected. </summary>
		public Color HeaderColorHighlighted { get; set; } = UnityColors.SceneMinorGridColor;
		public Color CellColorDefault { get; set; } = UnityColors.SelectionInactiveColor;
		public Color CellColorSelected { get; set; } = UnityColors.SelectionActiveColor;
		public Color CellErrorTint { get; set; } = Color.red;


		public TableEditor (ITableEditorCallbacks callbacks) {
			this.callbacks = callbacks;
			cellSize = new Vector2 (200f, EditorGUIUtility.singleLineHeight * 2);
			headerSize = new Vector2 (200f, EditorGUIUtility.singleLineHeight * 2);
		}


		public bool? DrawTable (Rect position) {
			if (callbacks == null) {
				return false;
			}

			var repaint = false;

			try {
				position.position = Vector2.zero;
				var borderArea = GetBorderPosition (position);
				var tableArea = GetTablePosition (position);
				var tableSize = new Vector2 (ColumnWidth * callbacks.ColumnCount, RowHeight * callbacks.RowCount);
				var colHeaderArea = new Rect (RowHeaderWidth, 0f, position.width - RowHeaderWidth - ScrollbarSize, ColumnHeaderHeight);
				var rowHeaderArea = new Rect (0f, ColumnHeaderHeight, RowHeaderWidth, position.height - ColumnHeaderHeight - ScrollbarSize);
				var e = Event.current;

				GUILayout.BeginArea (position);
				DrawScrollbarDecorators (position);
				DrawCorner (e, ref repaint);
				DrawColumnHeader (colHeaderArea, tableSize, e, ref repaint);
				DrawRowHeader (rowHeaderArea, tableSize, e, ref repaint);
				DrawTableCells (tableArea, tableSize, e, ref repaint);
				GUILayout.EndArea ();
			} catch {
				return null;
			}

			return repaint;
		}

		private void DrawScrollbarDecorators (Rect position) {
			var rightScrollbarDecoratorRect = new Rect (position.width - ScrollbarSize, 0f, ScrollbarSize, ColumnHeaderHeight);
			var bottomScrollbarDecoratorRect = new Rect (0f, position.height - ScrollbarSize, RowHeaderWidth, ScrollbarSize);

			EditorGUI.DrawRect (rightScrollbarDecoratorRect, UnityColors.SeparatorColor);
			EditorGUI.DrawRect (bottomScrollbarDecoratorRect, UnityColors.SeparatorColor);
			EditorGUI.LabelField (bottomScrollbarDecoratorRect, $"Size: X: {callbacks.ColumnCount} Y:{callbacks.RowCount} | Sel: X:{SelectedCell.x} X:{SelectedCell.y}");
		}

		private void DrawCorner (Event e, ref bool repaint) {
			if (callbacks is ITableEditorCallbacksExtended ext) {
				Rect cornerRect = new Rect (Vector2.zero, cellSize);
				_ = DrawRawCell (cornerRect, Color.clear, e, ref repaint);
				ext.OnDrawCorner (cornerRect);
			}
		}

		private void DrawColumnHeader (Rect position, Vector2 tableSize, Event e, ref bool repaint) {
			if (callbacks.ColumnCount == 0) {
				EditorGUI.DrawRect (position, HeaderColorDefault);
				return;
			}

			GUI.BeginScrollView (position, new Vector2 (scrollPosition.x, 0f), new Rect (0f, 0f, tableSize.x, 1f), false, false, GUIStyle.none, GUIStyle.none);
			for (int i = 0; i < callbacks.ColumnCount; i++) {
				var cellPos = new Rect (ColumnWidth * i, 0f, ColumnWidth, ColumnHeaderHeight);
				var cellCol = SelectedCell.x == i ? (SelectedCell.y == -1 ? CellColorSelected : HeaderColorHighlighted) : HeaderColorDefault;
				var tint = callbacks.HasCellError (new Vector2Int (i, -1)) ? CellErrorTint : Color.white;

				if (DrawRawCell (cellPos, cellCol * tint, e, ref repaint)) {
					SelectedCell = new Vector2Int (i, -1);
				}
				callbacks.OnDrawColumnHeader (i, cellPos);
				GUI.backgroundColor = Color.white;
			}
			GUI.EndScrollView ();
		}

		private void DrawRowHeader (Rect position, Vector2 tableSize, Event e, ref bool repaint) {
			if (callbacks.RowCount == 0) {
				EditorGUI.DrawRect (position, HeaderColorDefault);
				return;
			}

			GUI.BeginScrollView (position, new Vector2 (0f, scrollPosition.y), new Rect (0f, 0f, 1f, tableSize.y), false, false, GUIStyle.none, GUIStyle.none);
			for (int i = 0; i < callbacks.RowCount; i++) {
				var cellPos = new Rect (0f, RowHeight * i, RowHeaderWidth, RowHeight);
				var cellCol = SelectedCell.y == i ? (SelectedCell.x == -1 ? CellColorSelected : HeaderColorHighlighted) : HeaderColorDefault;
				var tint = callbacks.HasCellError (new Vector2Int(-1, i)) ? CellErrorTint : Color.white;

				if (DrawRawCell (cellPos, cellCol * tint, e, ref repaint)) {
					SelectedCell = new Vector2Int (-1, i);
				}
				callbacks.OnDrawRowHeader (i, cellPos);
			}
			GUI.EndScrollView ();
		}

		private void DrawTableCells (Rect position, Vector2 tableSize, Event e, ref bool repaint) {
			var cellPos = new Rect (0f, 0f, ColumnWidth, RowHeight);

			scrollPosition = GUI.BeginScrollView (position, scrollPosition, new Rect (Vector2.zero, tableSize), true, true);
			for (int x = 0; x < callbacks.ColumnCount; x++) {
				for (int y = 0; y < callbacks.RowCount; y++) {
					var cell = new Vector2Int (x, y);
					var cellCol = SelectedCell == cell ? CellColorSelected : CellColorDefault;

					GUI.backgroundColor = callbacks.HasCellError (cell) ? CellErrorTint : Color.white;
					if (DrawRawCell (cellPos, cellCol, e, ref repaint)) {
						SelectedCell = cell;
					}
					callbacks.OnDrawCell (cell, cellPos);
					GUI.backgroundColor = Color.white;

					cellPos.y += cellSize.y;
				}
				cellPos.y = 0f;
				cellPos.x += cellSize.x;
			}
			GUI.EndScrollView ();
		}

		private bool DrawRawCell (Rect cellPos, Color cellCol, Event e, ref bool repaint) {
			EditorGUI.DrawRect (cellPos, cellCol * GUI.backgroundColor);
			EditorDrawUtility.DrawSeparator (cellPos, EditorDrawUtility.Side.Right, 1f, UnityColors.SeparatorColor, true);
			EditorDrawUtility.DrawSeparator (cellPos, EditorDrawUtility.Side.Bottom, 1f, UnityColors.SeparatorColor, true);

			if (e.type == EventType.MouseDown && cellPos.Contains (e.mousePosition)) {
				repaint = true;
				return true;
			}
			return false;
		}

		private Rect GetBorderPosition (Rect position) {
			position.x += RowHeaderWidth;
			position.y += ColumnHeaderHeight;
			position.width -= RowHeaderWidth;
			position.height -= ColumnHeaderHeight;
			return position;
		}

		private Rect GetTablePosition (Rect position) {
			return GetBorderPosition (position);
		}
	}
}