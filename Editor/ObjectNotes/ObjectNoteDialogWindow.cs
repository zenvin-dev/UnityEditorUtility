using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.ObjectNotes {
	public class ObjectNoteDialogWindow : EditorWindow {

		private const float textAreaHeight = 100;

		private static GUIStyle textBoxStyle;
		private static GUIStyle TextBoxStyle {
			get {
				if (textBoxStyle == null) {
					textBoxStyle = new GUIStyle (EditorStyles.textArea) { clipping = TextClipping.Clip, wordWrap = true, };
				}
				return textBoxStyle;
			}
		}


		private NotesComponent target;
		private ObjectNote note;
		private bool editMode;

		[MenuItem("GameObject/Add Note", priority = 1200)]
		internal static void CreateNode () {
			var sel = Selection.activeGameObject;
			if (sel == null) {
				return;
			}

			if (!sel.TryGetComponent(out NotesComponent notes)) {
				notes = sel.AddComponent<NotesComponent> ();
				notes.hideFlags = HideFlags.HideInInspector;
			}

			Open (notes);
		}

		private static void Open (NotesComponent target) {
			if (target == null) {
				return;
			}
			var win = GetWindow<ObjectNoteDialogWindow> ();

			win.target = target;
			win.note = new ObjectNote ();
			win.editMode = false;

			win.titleContent = new GUIContent ("Add Object Note");

			var height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 + textAreaHeight;
			win.minSize = new Vector2 (200f, height);
			win.maxSize = new Vector2 (500f, height);

			win.ShowModalUtility ();
		}

		internal static void Edit (NotesComponent target, ObjectNote note) {
			if (note == null) {
				return;
			}
			var win = GetWindow<ObjectNoteDialogWindow> ();

			win.target = target;
			win.note = note;
			win.editMode = true;

			win.titleContent = new GUIContent ("Edit Object Note");

			float height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 1 + textAreaHeight;
			win.minSize = new Vector2 (200f, height);
			win.maxSize = new Vector2 (500f, height);

			win.ShowModalUtility ();
		}


		private void OnGUI () {
			if (note == null) {
				return;
			}

			note.Content = EditorGUILayout.TextArea (note.Content, TextBoxStyle, GUILayout.MinHeight (100));
			note.Type = (ObjectNote.NoteType)EditorGUILayout.EnumPopup ("Type", note.Type);

			if (!editMode && GUILayout.Button("Add Note")) {
				target.AddNote (note.Content, note.Type);
				ResetNote ();
				Close ();
			}
			if (editMode && target != null) {
				EditorUtility.SetDirty (target);
			}
		}

		private void ResetNote () {
			note = null;
		}

	}
}
