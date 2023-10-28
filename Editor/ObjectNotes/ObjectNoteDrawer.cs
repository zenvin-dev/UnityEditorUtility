using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.ObjectNotes {
	[InitializeOnLoad]
	internal static class ObjectNoteDrawer {

		static ObjectNoteDrawer () {
			Editor.finishedDefaultHeaderGUI += DrawHeader;
		}


		private static void DrawHeader (Editor editor) {
			if (editor.targets.Length > 1) {
				return;
			}

			if (editor.target is NotesComponent comp || (editor.target is GameObject go && go.TryGetComponent (out comp))) {
				if (Application.isPlaying) {
					return;
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Space (42);
				DrawHeaderControls (comp, editor);
				GUILayout.EndHorizontal ();
			}
		}

		private static void DrawHeaderControls (NotesComponent comp, Editor editor) {
			var e = Event.current;

			GUILayout.BeginVertical ();
			for (int i = 0; i < comp.NoteCount; i++) {
				var note = comp[i];
				
				EditorGUILayout.HelpBox (note.Content, ToMessageType (note.Type), true);
				var rect = GUILayoutUtility.GetLastRect ();
				if (e.type == EventType.ContextClick && rect.Contains(e.mousePosition)) {
					int j = i;
					OpenContext (rect, comp, j);
					editor.Repaint ();
				}
			}
			GUILayout.EndVertical ();
		}

		private static void OpenContext (Rect rect, NotesComponent comp, int i) {
			var gm = new GenericMenu ();

			gm.AddItem (new GUIContent ("Edit"), false, EditNote, (comp, comp[i]));
			gm.AddItem (new GUIContent ("Remove"), false, DeleteNote, (comp, i));

			gm.DropDown (rect);
		}

		private static void EditNote (object userData) {
			if (userData is (NotesComponent comp, ObjectNote note)) {
				ObjectNoteDialogWindow.Edit (comp, note);
			}
		}

		private static void DeleteNote (object userData) {
			if (userData is (NotesComponent comp, int index) && comp != null) {
				comp.DeleteNote (index);
				if (comp != null) {
					EditorUtility.SetDirty (comp);
				}
			}
		}

		private static MessageType ToMessageType (ObjectNote.NoteType type) {
			switch (type) {
				case ObjectNote.NoteType.Info:
					return MessageType.Info;
				case ObjectNote.NoteType.Warning:
					return MessageType.Warning;
				case ObjectNote.NoteType.Error:
					return MessageType.Error;
			}
			return MessageType.None;
		}
	}
}
