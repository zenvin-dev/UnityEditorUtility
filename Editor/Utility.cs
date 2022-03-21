using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Zenvin.EditorUtility {
	public static class Utility {

		private static readonly char[] Separators = new char[] { '/' };

		public static SerializedProperty FindSiblingProperty (this SerializedProperty prop, string name) {
			if (string.IsNullOrEmpty (name)) {
				return null;
			}

			string[] pathParts = prop.propertyPath.Split (Separators, System.StringSplitOptions.RemoveEmptyEntries);
			string path = "";
			for (int i = 0; i < pathParts.Length - 1; i++) {
				path = path + pathParts[i];
			}
			
			pathParts = name.Split (Separators, System.StringSplitOptions.RemoveEmptyEntries);
			
			path = path + "/" + pathParts[0];
			path = path.Trim ('/');

			SerializedProperty current = prop.serializedObject.FindProperty (path);
			if (current == null) {
				return null;
			}

			for (int i = 1; i < pathParts.Length; i++) {
				string _name = pathParts[i];

				if (current.propertyType == SerializedPropertyType.ObjectReference) {
					if (current.objectReferenceValue == null) {
						return null;
					}
					SerializedObject obj = new SerializedObject (current.objectReferenceValue);
					current = obj.FindProperty (pathParts[i]);
				} else {
					current = current.FindPropertyRelative (_name);
				}

				if (current == null) {
					return null;
				}
			}

			return current;
		}

	}
}