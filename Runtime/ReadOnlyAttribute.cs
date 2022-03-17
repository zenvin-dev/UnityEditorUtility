using UnityEngine;

namespace Zenvin.EditorUtility {
	public class ReadOnlyAttribute : PropertyAttribute {

		public readonly bool HasCondition = false;
		public readonly string ConditionProperty;
		public readonly object ConditionValue;
		public readonly bool InvertCondition = false;


		public ReadOnlyAttribute () { }

		public ReadOnlyAttribute (string conditionProperty, object conditionValue, bool invertCondition = false) {
			HasCondition = !string.IsNullOrEmpty (conditionProperty);
			ConditionProperty = conditionProperty;
			ConditionValue = conditionValue;
			InvertCondition = invertCondition;
		}
	}
}