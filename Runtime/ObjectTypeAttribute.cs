using System;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[AttributeUsage (AttributeTargets.Field, AllowMultiple = false)]
	public class ObjectTypeAttribute : PropertyAttribute {

		public readonly Type TypeOverride;


		public ObjectTypeAttribute (Type typeOverride) {
			TypeOverride = typeOverride;
		}
	}
}
