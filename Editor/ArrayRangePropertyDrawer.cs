using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtility {
	[CustomPropertyDrawer (typeof (ArrayRangeAttribute))]
	public class ArrayRangePropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			// draw original property if it's not an int
			if (property.propertyType != SerializedPropertyType.Integer) {
				base.OnGUI (position, property, label);
				return;
			}

			// get attribute
			ArrayRangeAttribute attr = attribute as ArrayRangeAttribute;
			if (attr == null) {
				base.OnGUI (position, property, label);
				return;
			}

			SerializedProperty arrProp = property.FindSiblingProperty (attr.ArrayProperty);

			// draw original property if array property wasn't found
			if (arrProp == null) {
				base.OnGUI (position, property, label);
				return;
			}

			int min = 0;
			int max = arrProp.arraySize - 1;
			int val = EditorGUI.IntSlider (position, label, property.intValue, Mathf.Min (min, max), Mathf.Max (min, max));
			property.intValue = val;
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight (property, label);
		}

	}
}