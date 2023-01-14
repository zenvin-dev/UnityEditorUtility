using UnityEngine;

namespace Zenvin.EditorUtil {
	public class ArrayRangeAttribute : PropertyAttribute {

		public readonly string ArrayProperty;

		public ArrayRangeAttribute (string arrayProperty) {
			ArrayProperty = arrayProperty;
		}
	}
}