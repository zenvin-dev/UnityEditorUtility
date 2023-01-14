using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Zenvin.EditorUtil {
	public sealed class SelectionCache<T> where T : Object {

		public enum ReferenceRetentionPolicy {
			None,
			DeselectOnNull,
			Keep
		}

		private T selected;
		private readonly Type targetType;

		public readonly bool MatchExactType;
		public readonly ReferenceRetentionPolicy RetentionPolicy;

		public T Selected {
			get {
				return selected;
			}
			private set {
				if (selected == value) {
					return;
				}
				selected = value;
				Changed = true;
			}
		}
		public bool Changed { get; private set; } = false;


		public SelectionCache () : this (false, ReferenceRetentionPolicy.Keep) { }

		public SelectionCache (bool matchExactType, ReferenceRetentionPolicy retentionPolicy) {
			MatchExactType = matchExactType;
			RetentionPolicy = retentionPolicy;
			targetType = typeof (T);

			Selection.selectionChanged += SelectionChangedHandler;
		}


		public void Acknowledge () {
			Changed = false;
		}


		private void SelectionChangedHandler () {
			Object sel = Selection.activeObject;
			bool typeMatch = sel == null || (MatchExactType && sel.GetType () == targetType) || sel is T;

			if (!typeMatch) {
				if (RetentionPolicy == ReferenceRetentionPolicy.None) {
					Selected = null;
				}
				return;
			}

			if (sel == null && RetentionPolicy == ReferenceRetentionPolicy.DeselectOnNull) {
				Selected = null;
				return;
			}

			if (sel != null) {
				Selected = sel as T;
			}
		}
	}
}