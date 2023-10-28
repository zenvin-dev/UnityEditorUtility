using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[DisallowMultipleComponent]
	public sealed class NotesComponent : MonoBehaviour {

#if UNITY_EDITOR
		[SerializeField] private List<ObjectNote> notes = new List<ObjectNote> ();


		internal int NoteCount => notes.Count;
		internal ObjectNote this[int index] => notes[index];


		internal void AddNote (string content, ObjectNote.NoteType type = ObjectNote.NoteType.Info) {
			if (string.IsNullOrWhiteSpace (content)) {
				return;
			}
			var note = new ObjectNote () { Content = content, Type = type };
			notes.Add (note);
		}

		internal void DeleteNote (int index) {
			if (index < 0 || index >= notes.Count) {
				return;
			}
			notes.RemoveAt (index);
			if (notes.Count == 0) {
				DestroyImmediate (this, false);
			}
		}
#endif
	}

	[Serializable]
	public class ObjectNote {
		public enum NoteType {
			Info,
			Warning,
			Error,
		}

		public NoteType Type = NoteType.Info;
		[TextArea] public string Content = "";
	}
}
