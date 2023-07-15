using Object = UnityEngine.Object;

using UnityEngine;
using UnityEditor;
using System;

namespace Zenvin.EditorUtil {
	public enum AssetAction {
		BeforeAddNested,
		AfterAddNested,
		BeforeDeleteNested,
	}

	public delegate void OnBeforeAddAsset<T> (T asset) where T : ScriptableObject;
	public delegate void OnAssetAction<T> (T asset, AssetAction action) where T : ScriptableObject;

	public static class AssetUtility {

		public static T CreateAsPartOf<T> (Object asset, OnBeforeAddAsset<T> onBeforeAdd = null) where T : ScriptableObject {
			return CreateAsPartOf (asset, typeof(T), onBeforeAdd);
		}

		public static T CreateAsPartOf<T> (Object asset, Type assetType, OnBeforeAddAsset<T> onBeforeAdd = null) where T : ScriptableObject {
			if (asset == null || assetType.IsAbstract) {
				return null;
			}

			T instance = ScriptableObject.CreateInstance (assetType) as T;
			if (instance == null) {
				return null;
			}
			onBeforeAdd?.Invoke (instance);
			AddObjectToAsset (instance, asset);
			
			return instance;
		}

		public static T CreateAsPartOf<T> (Object asset, OnAssetAction<T> onAction = null) where T : ScriptableObject {
			return CreateAsPartOf<T> (asset, typeof(T), onAction);
		}

		public static T CreateAsPartOf<T> (Object asset, Type assetType, OnAssetAction<T> onAction) where T : ScriptableObject {
			if (asset == null || assetType.IsAbstract) {
				return null;
			}

			T instance = ScriptableObject.CreateInstance (assetType) as T;
			if (instance == null) {
				return null;
			}
			onAction?.Invoke (instance, AssetAction.BeforeAddNested);
			AddObjectToAsset (instance, asset);
			onAction?.Invoke (instance, AssetAction.AfterAddNested);

			return instance;
		}

		public static bool DeleteSubAsset<TMain, TSub> (TSub asset, OnAssetAction<TSub> onAction = null) where TMain : Object where TSub : ScriptableObject {
			if (asset == null) {
				return false;
			}
			if (!AssetDatabase.IsSubAsset (asset)) {
				return false;
			}
			onAction?.Invoke (asset, AssetAction.BeforeDeleteNested);

			var main = GetMainAssetFromSubAsset<TMain, TSub> (asset);
			if (main != null) {
				Undo.RegisterCompleteObjectUndo (main, $"Delete from {main.name}");
			}

			Object.DestroyImmediate (asset, true);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			return true;
		}

		public static TMain GetMainAssetFromSubAsset<TMain, TSub> (TSub asset) where TMain : Object where TSub : Object {
			if (asset == null) {
				return null;
			}
			if (AssetDatabase.IsMainAsset(asset)) {
				return asset as TMain;
			}
			var path = AssetDatabase.GetAssetPath (asset);
			return AssetDatabase.LoadAssetAtPath<TMain> (path);
		}

		private static void AddObjectToAsset<T> (T instance, Object asset) where T : ScriptableObject {
			AssetDatabase.AddObjectToAsset (instance, asset);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

	}
}