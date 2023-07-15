using System;
using UnityEditor;
using UnityEngine;
using Zenvin.EditorUtil.ListTable;

namespace Zenvin.EditorUtil.NestedObjects {
	public interface INestedObjectsCallbacksBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		int GetChildCount (NestedObjectsEditorBase<TParent, TChild> source);
		TChild GetChild (NestedObjectsEditorBase<TParent, TChild> source, int index);

		void OnAssetAction (NestedObjectsEditorBase<TParent, TChild> source, TChild asset, AssetAction action);
		void OnContextClick (NestedObjectsEditorBase<TParent, TChild> source, TChild[] selected, GenericMenu menu);

		bool FilterNestedObjectType (NestedObjectsEditorBase<TParent, TChild> source, Type type);
		bool FilterChild (NestedObjectsEditorBase<TParent, TChild> source, string searchQuery, int index);

		void OnAddedChild (NestedObjectsEditorBase<TParent, TChild> source, TChild child);
		void OnRemoveChild (NestedObjectsEditorBase<TParent, TChild> source, int index);
	}

	public interface INestedObjectsCallbacks<TParent, TChild> : INestedObjectsCallbacksBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		GUIContent GetChildContent (NestedObjectsEditor<TParent, TChild> source, int index);
	}

	public interface INestedObjectsCallbacksExtended<TParent, TChild> : INestedObjectsCallbacksBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		GUIContent GetNoTargetContent (NestedObjectsEditorBase<TParent, TChild> source);
		GUIContent GetNoSelectionContent (NestedObjectsEditorBase<TParent, TChild> source);
	}

	public interface INestedObjectsTableCallbacks<TParent, TChild> : INestedObjectsCallbacksBase<TParent, TChild>
		where TParent : ScriptableObject
		where TChild : ScriptableObject {

		ColumnData[] GetColumns (NestedObjectsTableEditor<TParent, TChild> source);
		CellData[] GetChildRowContent (NestedObjectsTableEditor<TParent, TChild> source, int index);
	}
}