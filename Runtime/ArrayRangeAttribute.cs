using UnityEngine;

namespace Zenvin.EditorUtility {
	public class ArrayRangeAttribute : PropertyAttribute {

		public readonly string ArrayProperty;

		public ArrayRangeAttribute (string arrayProperty) {
			ArrayProperty = arrayProperty;
		}
	}
}