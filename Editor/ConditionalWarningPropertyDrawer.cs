using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil {
	[CustomPropertyDrawer (typeof (ConditionalWarningAttribute))]
	public class ConditionalWarningPropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			var attr = attribute as ConditionalWarningAttribute;
			GUIContent warning = new GUIContent (attr.Text);
			float height = EditorStyles.helpBox.CalcHeight (warning, position.width);

			Rect propRect = new Rect (position);
			propRect.height -= height;
			EditorGUI.PropertyField (propRect, property, label, true);

			Rect warnRect = new Rect (propRect);
			warnRect.y += propRect.height;
			warnRect.height = height;

			if (ShowWarning (property)) {
				EditorGUI.HelpBox (warnRect, warning.text, MessageType.Warning);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			float height = base.GetPropertyHeight (property, label);
			if (ShowWarning (property)) {
				height += EditorStyles.helpBox.CalcHeight (new GUIContent ((attribute as ConditionalWarningAttribute).Text), EditorGUIUtility.currentViewWidth);
			}
			return height;
		}


		private bool ShowWarning (SerializedProperty prop) {
			var attr = attribute as ConditionalWarningAttribute;
			return attr.Invert && prop.CompareValue (attr.Value);
		}

	}
}