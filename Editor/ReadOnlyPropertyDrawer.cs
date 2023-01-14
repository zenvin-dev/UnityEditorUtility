using UnityEngine;
using UnityEditor;

namespace Zenvin.EditorUtil {
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
				enabled = condProp.CompareValue (attr.ConditionValue);
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