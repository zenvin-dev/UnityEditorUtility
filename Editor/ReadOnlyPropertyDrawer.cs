using UnityEngine;
using UnityEditor;

namespace Zenvin.EditorUtility {
	[CustomPropertyDrawer (typeof (ReadOnlyAttribute))]
	public class ReadOnlyPropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {

			ReadOnlyAttribute attr = attribute as ReadOnlyAttribute;
			if (attr == null) {
				base.OnGUI (position, property, label);
				return;
			}

			bool enabled = false;

			if (attr.HasCondition) {
				SerializedProperty condProp = property.FindSiblingProperty (attr.ConditionProperty);
				if (condProp != null) {
					switch (condProp.propertyType) {
						case SerializedPropertyType.Boolean:
							enabled = condProp.boolValue.Equals (attr.ConditionValue);
							break;
						case SerializedPropertyType.Float:
							enabled = condProp.floatValue.Equals (attr.ConditionValue);
							break;
						case SerializedPropertyType.String:
							enabled = condProp.stringValue.Equals (attr.ConditionValue);
							break;
						case SerializedPropertyType.Integer:
							enabled = condProp.intValue.Equals (attr.ConditionValue);
							break;
						case SerializedPropertyType.Enum:
							enabled = condProp.enumValueIndex.Equals (attr.ConditionValue);
							break;
						case SerializedPropertyType.Character:
							if (condProp.stringValue.Length > 0) {
								enabled = condProp.stringValue[0].Equals (attr.ConditionValue);
							}
							break;
					}
				}
			}

			if (attr.InvertCondition) {
				enabled = !enabled;
			}

			GUI.enabled = enabled;
			//base.OnGUI (position, property, label);
			EditorGUI.PropertyField (position, property, label);
			GUI.enabled = true;

		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight (property, label);
		}

	}
}