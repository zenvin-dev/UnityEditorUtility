using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[CustomPropertyDrawer (typeof (UniqueAttribute))]
	public class UniquePropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.serializedObject.targetObjects.Length != 1) {
				EditorGUI.LabelField (EditorGUI.PrefixLabel (position, label), "Cannot edit multiple unique values.");
				return;
			}

			if (property.propertyType != SerializedPropertyType.String && property.propertyType != SerializedPropertyType.Integer) {
				base.OnGUI (position, property, label);
				return;
			}

			if (DrawField (position, property, label) && !IsPropertyValueUnique (property)) {
				UniqueModalDialog.OpenForProperty (property);
			}
		}

		private bool DrawField (Rect position, SerializedProperty property, GUIContent label) {
			switch (property.propertyType) {
				case SerializedPropertyType.String:
					string prevStr = property.stringValue;
					EditorGUI.DelayedTextField (position, property, label);
					return prevStr != property.stringValue;
				case SerializedPropertyType.Integer:
					int prevInt = property.intValue;
					EditorGUI.DelayedIntField (position, property, label);
					return prevInt != property.intValue;
			}
			return false;
		}

		private static bool IsPropertyValueUnique (SerializedProperty prop) {
			var targets = Utility.FindAll (prop.serializedObject.targetObject.GetType ());
			foreach (var t in targets) {
				if (t == prop.serializedObject.targetObject) {
					continue;
				}
				if (prop.CompareValue (new SerializedObject (t).FindProperty (prop.propertyPath))) {
					return false;
				}
			}
			return true;
		}

	}
}