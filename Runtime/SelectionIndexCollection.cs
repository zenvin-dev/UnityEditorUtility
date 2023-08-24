using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.EditorUtil {
	public class SelectionIndexCollection : IEnumerable<int> {

		public delegate void OnSelectionChanged ();

		public event OnSelectionChanged SelectionChanged;

		private readonly List<int> values = new List<int> ();
		private int? lastAdded = null;

		public int SelectionCount => values.Count;
		public int? ActiveSelection => lastAdded.HasValue ? lastAdded.Value : -1;

		/// <summary>
		/// Selects the given index.
		/// </summary>
		/// <param name="index"> The index to select. Cannot be less than 0. </param>
		/// <param name="bridge"> Whether the selection between this and the previous should be bridged (Shift-Key). </param>
		/// <param name="append"> Whether the selection should be appended to the current (Ctrl-Key). </param>
		public void Select (int index, bool bridge, bool append) {
			if (index < 0) {
				return;
			}

			if (append) {
				if (Toggle (index)) {
					lastAdded = index;
				}
				SelectionChanged?.Invoke ();
				return;
			}

			if (bridge) {
				if (!lastAdded.HasValue || lastAdded.Value == index) {
					lastAdded = index;
					if (Toggle (index, true)) {
						SelectionChanged?.Invoke ();
					}
					return;
				}

				var val = lastAdded.Value;
				var min = Mathf.Min (val, index);
				var max = Mathf.Max (val, index);
				for (int i = min; i <= max; i++) {
					Toggle (i, true);
				}
				lastAdded = index;
				SelectionChanged?.Invoke ();
				return;
			}

			ClearInternal ();
			Toggle (index, true);
			lastAdded = index;
			SelectionChanged?.Invoke ();
		}

		/// <summary>
		/// Selects or deselects a specific index.
		/// </summary>
		public void SetSelected (int index, bool state) {
			if (index < 0) {
				return;
			}
			if (Toggle (index, state)) {
				SelectionChanged?.Invoke ();
			}
		}

		/// <summary>
		/// Selects or deselects a range of indices.
		/// </summary>
		public void SetRangeSelected (IEnumerable<int> indices, bool state) {
			if (indices == null) {
				return;
			}
			foreach (int i in indices) {
				if (i < 0) {
					continue;
				}
				Toggle (i, state);
			}
			SelectionChanged?.Invoke ();
		}

		/// <summary>
		/// Selects only the next index, based on the last active one.
		/// </summary>
		public void SelectNext (int? max = null) {
			if (!lastAdded.HasValue) {
				Select (0, false, false);
			} else {
				var index = max.HasValue ? Mathf.Clamp (lastAdded.Value + 1, 0, max.Value) : lastAdded.Value + 1;
				Select (index, false, false);
			}
		}

		/// <summary>
		/// Selects only the previous index, based on the last active one.
		/// </summary>
		public void SelectPrevious (int? max = null) {
			if (!lastAdded.HasValue) {
				ClearInternal ();
				Toggle (0, true);
				return;
			}
			int index = max.HasValue ? Mathf.Clamp (lastAdded.Value, 0, max.Value) : lastAdded.Value + 1;
			Select (index, false, false);
		}

		/// <summary>
		/// Clears the current selection.
		/// </summary>
		public void Clear () {
			ClearInternal ();
			SelectionChanged?.Invoke ();
		}

		/// <summary>
		/// Returns a copy of all currently selected indices.
		/// </summary>
		public int[] GetSelected () {
			return values.ToArray ();
		}

		/// <summary>
		/// Returns whether the given index is selected.
		/// </summary>
		public bool Contains (int index) {
			return values.Contains (index);
		}


		private void ClearInternal () {
			values.Clear ();
			lastAdded = null;
		}

		private bool Toggle (int index) {
			if (values.Contains (index)) {
				values.Remove (index);
				return false;
			} else {
				values.Add (index);
				return true;
			}
		}

		private bool Toggle (int index, bool state) {
			if (state && !values.Contains (index)) {
				values.Add (index);
				return true;
			}
			if (!state && values.Contains (index)) {
				values.Remove (index);
				return true;
			}
			return false;
		}


		public IEnumerator<int> GetEnumerator () => ((IEnumerable<int>)values).GetEnumerator ();
		IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable)values).GetEnumerator ();
	}
}