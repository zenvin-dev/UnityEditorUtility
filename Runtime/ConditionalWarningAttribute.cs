using UnityEngine;

namespace Zenvin.EditorUtil {
	
	public sealed class ConditionalWarningAttribute : PropertyAttribute {

		public readonly string ConditionProperty;
		public readonly object Value;
		public readonly bool Invert;
		public readonly string Text;


		public ConditionalWarningAttribute (string conditionProperty, object value, string text) : this (conditionProperty, value, text, false) { }

		public ConditionalWarningAttribute (string conditionProperty, object value, string text, bool invert) {
			ConditionProperty = conditionProperty;
			Value = value;
			Invert = invert;
			Text = text;
		}
	}
}