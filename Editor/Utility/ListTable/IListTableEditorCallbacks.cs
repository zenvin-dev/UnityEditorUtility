namespace Zenvin.EditorUtil.ListTable {
	public interface IListTableEditorCallbacks {
		int GetRowCount (ListTableEditor source);
		RowData GetRowContent (ListTableEditor source, int row);
		void OnClickCell (ListTableEditor source, int row, int column, bool context);
	}
}