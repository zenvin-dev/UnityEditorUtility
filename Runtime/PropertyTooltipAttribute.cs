using System;

namespace Zenvin.EditorUtil {
	/// <summary>
	/// Attribute that can be used to add tooltips to properties.
	/// </summary>
	[AttributeUsage (AttributeTargets.Property)]
	public class PropertyTooltipAttribute : Attribute {
		public readonly string Tooltip;

		private PropertyTooltipAttribute () { }

		public PropertyTooltipAttribute (string tooltip) {
			Tooltip = tooltip;
		}
	}
}