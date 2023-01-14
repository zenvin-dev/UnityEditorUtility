using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[CustomPropertyDrawer (typeof (ObjectFlagsAttribute))]
	public class ObjectFlagsDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			ObjectFlagsAttribute attr = attribute as ObjectFlagsAttribute;

			if (property.propertyType != SerializedPropertyType.ObjectReference || attr == null) {
				base.OnGUI (position, property, label);
				return;
			}

			Object obj = property.objectReferenceValue;
			EditorGUI.PropertyField (position, property, label);
			Object cur = property.objectReferenceValue;
			
			if (obj != cur) {
				if (obj != null) {
					obj.hideFlags = HideFlags.None;
				}
				if (cur != null) {
					cur.hideFlags = attr.Flags;
				}
			}

		}

	}
}