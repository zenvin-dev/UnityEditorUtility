using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.NestedObjects {
	public abstract class NestedObjectsEditorBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		private TParent target;
		
		protected SerializedObject targetObject = null;
		protected Editor targetEditor = null;


		public SelectionIndexCollection Selected { get; private set; } = new SelectionIndexCollection ();
		public TParent Target {
			get {
				return target;
			}
			set {
				if (target == value) {
					return;
				}
				target = value;
				Selected.Clear ();
				if (target == null) {
					targetObject = null;
				} else {
					targetObject = new SerializedObject (target);
				}
			}
		}
	}

	public abstract class NestedObjectsEditorBase<TCallbacks, TParent, TChild> : NestedObjectsEditorBase<TParent, TChild>
		where TCallbacks : INestedObjectsCallbacksBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		protected readonly TCallbacks Callbacks;

		private static readonly float ToolbarHeight = EditorGUIUtility.singleLineHeight;
		private static readonly Rect[] positions = new Rect[2];
		private readonly List<int> searchResults = new List<int> ();

		private GUIContent addItemContent;
		private Type lastType = null;

		private Vector2 editorScroll;

		protected GenericMenu Menu { get; set; }
		protected string SearchQuery { get; private set; }
		protected int SearchResultCount => searchResults?.Count ?? 0;
		protected GUIContent AddItemContent => addItemContent == null ? addItemContent = EditorGUIUtility.IconContent ("d_Toolbar Plus") : addItemContent;


		public NestedObjectsEditorBase (TCallbacks callbacks) {
			Callbacks = callbacks;
			Selected.SelectionChanged += UpdateEditedObject;
		}


		public bool Draw (Rect position) {
			if (Callbacks == null) {
				return false;
			}

			if (Target == null || targetObject == null) {
				var label = Callbacks is INestedObjectsCallbacksExtended<TParent, TChild> ext
						? ext.GetNoTargetContent (this)
						: new GUIContent ($"Select [{typeof (TParent)}] asset to edit.");
				GUI.Box (position, label, new GUIStyle (EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
				return false;
			}

			if (Menu != null && Menu.GetItemCount () > 0) {
				Menu.ShowAsContext ();
			}
			Menu = null;

			position.SplitNonAlloc (RectTransform.Axis.Horizontal, positions);
			positions[0] = positions[0].Inset (RectTransform.Edge.Top, ToolbarHeight, out Rect toolbarRect);
			EditorDrawUtility.DrawSeparator (toolbarRect, EditorDrawUtility.Side.Top, 1f, UnityColors.SeparatorColor, false);

			DrawToolbar (toolbarRect);
			DrawList (positions[0]);
			DrawEditor (positions[1]);

			return Menu != null && Menu.GetItemCount () > 0;
		}


		protected abstract void DrawToolbar (Rect position);

		protected abstract void DrawList (Rect position);

		protected virtual void DrawEditor (Rect position) {
			if (targetEditor == null || targetEditor.target == null) {
				var label = Callbacks is INestedObjectsCallbacksExtended<TParent, TChild> ext
					? ext.GetNoSelectionContent (this)
					: new GUIContent ($"Select item(s) from list to edit.");
				GUI.Box (position, label, new GUIStyle (EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter });
				return;
			}

			GUILayout.BeginArea (position);
			targetEditor.DrawHeader ();
			editorScroll = GUILayout.BeginScrollView (editorScroll);

			targetEditor.DrawDefaultInspector ();

			GUILayout.EndArea ();
			GUILayout.EndArea ();
		}


		protected int GetIndex (int index) {
			return searchResults == null || index < 0 || index >= searchResults.Count ? index : searchResults[index];
		}

		protected void SetSearchQuery (string value) {
			if (value == SearchQuery) {
				return;
			}
			UpdateSearchResults (value);
		}

		protected void CreateAddItemMenu (Rect position) {
			position.width = 400f;
			var gm = new GenericMenu ();

			if (lastType != null && Callbacks.FilterNestedObjectType(this, lastType)) {
				gm.AddItem (new GUIContent ($"Last used: {lastType.FullName}"), false, OnAddItem, lastType);
				gm.AddSeparator ("");
			}

			var cType = typeof (TChild);
			if (IsTypeValid (cType)) {
				
				gm.AddItem (new GUIContent (cType.FullName.Replace (".", "/")), false, OnAddItem, cType);
			}

			var types = TypeCache.GetTypesDerivedFrom<TChild> ();
			foreach (var t in types) {
				if (IsTypeValid(t)) {
					var type = t;
					gm.AddItem (new GUIContent (t.FullName.Replace (".", "/")), false, OnAddItem, type);
				}
			}

			gm.DropDown (position);
		}

		protected TChild[] GetSelectedChildren () {
			if (Callbacks == null) {
				return null;
			}
			var targets = new List<TChild> (Selected.SelectionCount);
			foreach (var i in Selected) {
				var child = Callbacks.GetChild (this, i);
				if (child != null) {
					targets.Add (child);
				}
			}
			return targets.ToArray ();
		}

		protected bool IsTypeValid (Type type) {
			var cType = typeof (TChild);
			return type != null &&
				   !type.IsAbstract &&
				   !type.IsGenericTypeDefinition &&
				   (type.IsSubclassOf (cType) || type == cType) &&
				   Callbacks.FilterNestedObjectType (this, type);
		}

		protected void OnAddItem (object rawType) {
			if (rawType is Type type && targetObject != null) {
				var item = AssetUtility.CreateAsPartOf<TChild> (Target, type, OnAssetAction);
				targetObject.ApplyModifiedPropertiesWithoutUndo ();
				Callbacks.OnAddedChild (this, item);
				targetObject.ApplyModifiedPropertiesWithoutUndo ();
				EditorUtility.SetDirty (Target);
				AssetDatabase.SaveAssetIfDirty (Target);

				lastType = type;
			}
		}

		protected void OnDeleteSelected () {
			if (Target == null || Selected == null || Selected.SelectionCount == 0) {
				return;
			}

			var indices = new List<int> (Selected);
			indices.Sort ();
			for (int i = indices.Count - 1; i >= 0; i--) {
				var index = indices[i];
				var child = Callbacks.GetChild (this, index);
				if (child == null) {
					continue;
				}
				Callbacks.OnRemoveChild (this, index);
				AssetUtility.DeleteSubAsset<TParent, TChild> (child);
			}
			Selected.Clear ();
		}

		protected void OnAssetAction (TChild asset, AssetAction action) {
			Callbacks?.OnAssetAction (this, asset, action);
		}

		private void UpdateEditedObject () {
			if (Callbacks == null) {
				return;
			}
			var targets = new List<TChild> (Selected.SelectionCount);
			foreach (var i in Selected) {
				var child = Callbacks.GetChild (this, i);
				if (child != null) {
					targets.Add (child);
				}
			}
			targetEditor = Editor.CreateEditor (targets.ToArray ());
		}

		private void UpdateSearchResults (string search) {
			SearchQuery = search;
			searchResults.Clear ();

			if (string.IsNullOrEmpty (SearchQuery)) {
				return;
			}

			searchResults.Clear ();
			var count = Callbacks.GetChildCount (this);
			for (int i = 0; i < count; i++) {
				if (Callbacks.FilterChild (this, SearchQuery, i)) {
					searchResults.Add (i);
				} else {
					Selected.SetSelected (i, false);
				}
			}
		}
	}
}