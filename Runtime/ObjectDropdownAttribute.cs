using System;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[AttributeUsage (AttributeTargets.Field)]
	public class ObjectDropdownAttribute : PropertyAttribute {
		public readonly bool AllowSubAssets;


		public ObjectDropdownAttribute () { }

		public ObjectDropdownAttribute (bool allowSubAssets) {
			AllowSubAssets = allowSubAssets;
		}
	}
}