using System;
using UnityEditor;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Zenvin.EditorUtil {
	[CustomPropertyDrawer (typeof (ObjectDropdownAttribute))]
	public class ObjectDropdownPropertyDrawer : PropertyDrawer {

		private static readonly Type ScriptableObjectType = typeof (ScriptableObject);

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			if (property.propertyType != SerializedPropertyType.ObjectReference || !ScriptableObjectType.IsAssignableFrom (fieldInfo.FieldType)) {
				base.OnGUI (position, property, label);
				return;
			}

			string content = $"{(property.objectReferenceValue == null ? $"None" : property.objectReferenceValue.name)} ({fieldInfo.FieldType.Name})";
			Rect fieldRect = EditorGUI.PrefixLabel (position, label);
			if (GUI.Button (fieldRect, content, EditorStyles.popup)) {
				OpenDropdown (fieldRect, property);
			}
		}

		private void OpenDropdown (Rect rect, SerializedProperty property) {
			UObject[] allObjects = Utility.FindAll (fieldInfo.FieldType);

			GenericMenu menu = new GenericMenu ();
			menu.allowDuplicateNames = true;
			menu.AddItem (new GUIContent ("None"), property.objectReferenceValue == null, OnSelectOption, new EntryData (property, null));
			for (int i = 0; i < allObjects.Length; i++) {
				if (allObjects[i] is ScriptableObject so) {
					menu.AddItem (new GUIContent (so.name), property.objectReferenceValue == so, OnSelectOption, new EntryData (property, so));
				}
			}
			menu.DropDown (rect);
		}

		private void OnSelectOption (object value) {
			if (value is EntryData data) {
				data.Property.objectReferenceValue = data.Value;
				data.Property.serializedObject.ApplyModifiedProperties ();
			}
		}


		private class EntryData {
			public readonly SerializedProperty Property;
			public readonly ScriptableObject Value;

			public EntryData (SerializedProperty property, ScriptableObject value) {
				Property = property;
				Value = value;
			}
		}

	}
}