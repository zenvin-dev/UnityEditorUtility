using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Zenvin.EditorUtility {
	public static class Utility {

		public static SerializedProperty FindSiblingProperty (this SerializedProperty prop, string name) {
			if (string.IsNullOrEmpty (name)) {
				return null;
			}

			string[] pathParts = prop.propertyPath.Split ('/');
			string path = "";
			for (int i = 0; i < pathParts.Length - 1; i++) {
				path = path + pathParts[i];
			}
			path = path + "/" + name;
			path = path.Trim ('/');

			return prop.serializedObject.FindProperty (path);
		}

	}
}