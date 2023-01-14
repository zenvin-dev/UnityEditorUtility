using UnityEngine;

namespace Zenvin.EditorUtil {
	public class ObjectFlagsAttribute : PropertyAttribute {

		public readonly HideFlags Flags;

		public ObjectFlagsAttribute (HideFlags flags) {
			Flags = flags;
		}

	}
}