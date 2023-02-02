using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenvin.EditorUtil.Sidebar {
	public abstract class ButtonBar {

		protected const int ButtonPadding = 5;
		protected const int ButtonSpacing = 0;
		protected const int SeparatorSpacing = 2;
		protected const float SeparatorHeight = 1f;
		protected const float SeparatorMargin = 5f;

		private static GUIStyle buttonStyle;
		protected static GUIStyle ButtonStyle {
			get {
				if (buttonStyle == null) {
					buttonStyle = new GUIStyle (EditorStyles.label);
					buttonStyle.margin = new RectOffset ();
					buttonStyle.padding = new RectOffset (ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding);
					buttonStyle.hover.background = Utility.GetColorTexture (UnityColors.SelectionInactiveColor);
				}
				return buttonStyle;
			}
		}
		private static GUIStyle buttonStyleActive;
		protected static GUIStyle ButtonStyleActive {
			get {
				if (buttonStyleActive == null) {
					buttonStyleActive = new GUIStyle (ButtonStyle);

					Texture2D tex = new Texture2D (1, 1);
					tex.SetPixel (0, 0, UnityColors.SelectionActiveColor);
					tex.Apply ();

					buttonStyleActive.normal.background = tex;
					buttonStyleActive.active.background = tex;
					buttonStyleActive.focused.background = tex;
					buttonStyleActive.hover.background = tex;
				}
				return buttonStyleActive;
			}
		}
		private static GUIStyle tooltipLabelStyle;
		protected static GUIStyle TooltipLabelStyle {
			get {
				if (tooltipLabelStyle == null) {
					tooltipLabelStyle = new GUIStyle (EditorStyles.label);
					tooltipLabelStyle.margin = new RectOffset (10, 10, 0, 0);
					tooltipLabelStyle.alignment = TextAnchor.MiddleLeft;
				}
				return tooltipLabelStyle;
			}
		}

		protected readonly ISidebarCallbacks callbacks;
		protected readonly List<SidebarButton> tabs = new List<SidebarButton> ();
		protected readonly List<SidebarButton> buttons = new List<SidebarButton> ();

		protected TooltipContent ActiveTooltip { get; set; }


		public ButtonBar (ISidebarCallbacks callbacks) => this.callbacks = callbacks;


		public abstract bool Draw (Rect position);

		public abstract void DrawTooltip (Rect sidebarPosition);


		protected class SidebarButton {
			public readonly string Label;
			public readonly Texture2D Icon;

			public SidebarButton (string label, Texture2D icon) {
				Label = label;
				Icon = icon;
			}
		}

		protected class TooltipContent {
			public readonly Rect TargetPosition;
			public readonly string Label;

			public TooltipContent (Rect position, string label) {
				TargetPosition = position;
				Label = label;
			}
		}
	}
}