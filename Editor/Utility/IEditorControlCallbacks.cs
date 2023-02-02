namespace Zenvin.EditorUtil {
	public interface IEditorControlCallbacks {
		void QueueRepaint ();
		void ForceRepaint ();
	}
}