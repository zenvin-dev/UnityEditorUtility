using UnityEngine;

namespace Zenvin.EditorUtility {
	public class ObjectFlagsAttribute : PropertyAttribute {

		public readonly HideFlags Flags;

		public ObjectFlagsAttribute (HideFlags flags) {
			Flags = flags;
		}

	}
}