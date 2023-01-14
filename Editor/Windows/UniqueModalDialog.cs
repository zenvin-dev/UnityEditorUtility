using System;
using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	public class UniqueModalDialog : EditorWindow {

		private SerializedProperty prop;

		private int intValue;
		private string stringValue;
		private bool wasNotUnique;

		internal static void OpenForProperty (SerializedProperty prop) {
			if (prop == null) {
				return;
			}
			if (prop.propertyType != SerializedPropertyType.String && prop.propertyType != SerializedPropertyType.Integer) {
				return;
			}

			UniqueModalDialog dialog = CreateWindow<UniqueModalDialog> ();
			dialog.prop = prop;
			dialog.minSize = new Vector2 (700f, 100f);
			dialog.maxSize = new Vector2 (700f, 100f);
			dialog.titleContent = new GUIContent ("Unique Value");

			dialog.SetupPropertyValues ();
			dialog.ShowModalUtility ();
		}

		private void SetupPropertyValues () {
			switch (prop.propertyType) {
				case SerializedPropertyType.String:
					stringValue = prop.stringValue;
					break;
				case SerializedPropertyType.Integer:
					intValue = prop.intValue;
					break;
			}

		}

		private void OnGUI () {
			if (prop == null) {
				Close ();
				return;
			}

			try {
				EditorGUILayout.LabelField (
					$"Enter unique value for '{prop.displayName}' on {prop.serializedObject.targetObject.name} ({prop.serializedObject.targetObject.GetType ().FullName})",
					new GUIStyle (EditorStyles.label) { richText = true }
				);

				bool changed = false;
				switch (prop.propertyType) {
					case SerializedPropertyType.Integer:
						changed = DrawIntField ();
						break;
					case SerializedPropertyType.String:
						changed = DrawStringField ();
						break;
				}

				if (wasNotUnique) {
					EditorGUILayout.HelpBox ("Entered value was not unique.", MessageType.Error);
				} else if (changed) {
					ApplyValue ();
					Close ();
				}
			} catch {
				Close ();
			}
		}

		private bool DrawIntField () {
			int val = EditorGUILayout.DelayedIntField ("Value", intValue);
			if (val != intValue) {
				intValue = val;

				bool unique = IsPropertyValueUnique (prop, val);
				wasNotUnique = !unique;

				return true;
			}
			return false;
		}

		private bool DrawStringField () {
			string val = EditorGUILayout.DelayedTextField ("GUID", stringValue);
			if (GUILayout.Button ("Generate random GUID")) {
				val = Guid.NewGuid ().ToString ();
			}
			if (val != stringValue) {
				stringValue = val;

				bool unique = IsPropertyValueUnique (prop, val);
				wasNotUnique = !unique;

				return true;
			}
			return false;
		}

		private void ApplyValue () {
			switch (prop.propertyType) {
				case SerializedPropertyType.Integer:
					prop.intValue = intValue;
					break;
				case SerializedPropertyType.String:
					prop.stringValue = stringValue;
					break;
			}
			prop.serializedObject.ApplyModifiedPropertiesWithoutUndo ();
		}


		private static bool IsPropertyValueUnique (SerializedProperty prop, object value) {
			var targets = Utility.FindAll (prop.serializedObject.targetObject.GetType ());
			foreach (var t in targets) {
				if (t == prop.serializedObject.targetObject) {
					continue;
				}
				if (prop.CompareValue (value)) {
					return false;
				}
			}
			return true;
		}

	}
}