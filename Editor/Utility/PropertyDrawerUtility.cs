using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	public static class PropertyDrawerUtility {

		public enum InfoPlacement {
			Start,
			End,
		}

		public enum InfoMode {
			Tooltip,
			Popup,
			Both,
		}

		private static GUIContent infoIcon;
		private static GUIContent InfoIcon { get => infoIcon == null ? (infoIcon = EditorGUIUtility.IconContent ("_Help@2x")) : infoIcon; }


		public static Rect DrawInfo (Rect position, string info, InfoPlacement placement = InfoPlacement.End, InfoMode mode = InfoMode.Tooltip) {
			if (string.IsNullOrWhiteSpace (info)) {
				return position;
			}

			var tooltipContent = mode == InfoMode.Popup ? "" : info;
			var content = new GUIContent (InfoIcon) { tooltip = tooltipContent };
			var insetEdge = placement == InfoPlacement.Start ? RectTransform.Edge.Left : RectTransform.Edge.Right;

			position = position.Inset (insetEdge, EditorGUIUtility.singleLineHeight, out var btnRect);
			btnRect.height = EditorGUIUtility.singleLineHeight;

			if (GUI.Button(btnRect, content, GUIStyle.none) && mode != InfoMode.Tooltip) {
				EditorUtility.DisplayDialog ("Info", info, "Close");
			}

			return position;
		}

	}
}