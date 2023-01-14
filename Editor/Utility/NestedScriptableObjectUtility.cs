using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Zenvin.EditorUtil {
	public static class NestedScriptableObjectUtility {

		public static bool CreateNestedInstance<T> (ScriptableObject parent, out T nested, string name = null) where T : ScriptableObject {
			nested = null;

			if (parent == null) {
				Debug.LogError ($"Argument was null: {nameof (parent)}");
				return false;
			}
			if (AssetDatabase.IsSubAsset (parent)) {
				Debug.LogError ("Cannot create SubAsset of SubAsset.");
				return false;
			}
			if (!AssetDatabase.IsMainAsset (parent)) {
				Debug.LogError ("Cannot create SubAsset, if parent is not an Asset.");
				return false;
			}
			if (EditorApplication.isPlaying) {
				Debug.LogError ("Cannot create SubAsset while game is running.");
				return false;
			}

			var instance = ScriptableObject.CreateInstance<T> ();
			instance.name = string.IsNullOrWhiteSpace (name) ? Guid.NewGuid ().ToString () : name;

			AssetDatabase.AddObjectToAsset (instance, parent);
			AssetDatabase.SaveAssetIfDirty (instance);
			AssetDatabase.SaveAssetIfDirty (parent);
			AssetDatabase.Refresh ();

			nested = instance;
			return true;
		}

		public static bool DestroyNestedInstance (ScriptableObject obj) {
			if (obj == null) {
				Debug.LogError ($"Argument was null: {nameof (obj)}");
				return false;
			}
			if (!AssetDatabase.IsSubAsset (obj)) {
				Debug.LogError ("Given object is not a SubAsset.");
				return false;
			}
			if (EditorApplication.isPlaying) {
				Debug.LogError ("Cannot delete asset while game is running.");
				return false;
			}

			Object.DestroyImmediate (obj, true);

			AssetDatabase.Refresh ();
			AssetDatabase.SaveAssets ();

			return true;
		}

	}
}