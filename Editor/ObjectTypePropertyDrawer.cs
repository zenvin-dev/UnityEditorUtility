using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[CustomPropertyDrawer (typeof (ObjectTypeAttribute))]
	public class ObjectTypePropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.propertyType != SerializedPropertyType.ObjectReference) {
				EditorGUI.PropertyField (position, property, label);
				return;
			}
			if (!(attribute is ObjectTypeAttribute attr)) {
				EditorGUI.PropertyField (position, property, label);
				return;
			}
			if (attr.TypeOverride == null || !attr.TypeOverride.IsInterface) {
				EditorGUI.PropertyField (position, property, label);
				return;
			}
			EditorGUI.ObjectField (position, property, attr.TypeOverride, label);
		}

	}
}