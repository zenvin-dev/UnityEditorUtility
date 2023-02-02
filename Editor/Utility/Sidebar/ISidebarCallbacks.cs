using Zenvin.EditorUtil.Sidebar;

namespace Zenvin.EditorUtil.Sidebar {
	public interface ISidebarCallbacks : IEditorControlCallbacks {
		void TabChanged (ButtonSidebar sidebar, int index, int previousIndex);
		void ButtonClicked (ButtonSidebar sidebar, int index);
		bool ButtonEnabled (ButtonSidebar sidebar, int index);
	}

}