using UnityEngine;

namespace Zenvin.EditorUtil.Tabs {
	public interface ITabsEditorCallbacks {
		string[] GetTabNames (TabsEditor source);
		void OnDrawTab (TabsEditor source, Rect position, int index);
	}
}