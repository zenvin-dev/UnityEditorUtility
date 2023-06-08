using UnityEngine;

namespace Zenvin {
	public static class RectExtensions {

		public static Rect Inset (this Rect rect, float amount) {
			return rect.Inset (amount, amount, amount, amount);
		}

		public static Rect Inset (this Rect rect, float left, float right, float top, float bottom) {
			rect = new Rect (rect);

			rect.x += left;
			rect.y += top;
			rect.width -= left + right;
			rect.height -= top + bottom;

			return rect;
		}

		public static Rect Inset (this Rect rect, RectTransform.Edge edge, float amount, out Rect inset) {
			switch (edge) {
				case RectTransform.Edge.Left:
					inset = new Rect (new Vector2(rect.x, rect.y), new Vector2(amount, rect.height));
					return rect.Inset (amount, 0, 0, 0);
				case RectTransform.Edge.Right:
					inset = new Rect (new Vector2(rect.x + rect.width - amount, rect.y), new Vector2(amount, rect.height));
					return rect.Inset (0, amount, 0, 0);
				case RectTransform.Edge.Top:
					inset = new Rect (new Vector2(rect.x, rect.y), new Vector2(rect.width, amount));
					return rect.Inset (0, 0, amount, 0);
				case RectTransform.Edge.Bottom:
					inset = new Rect (new Vector2(rect.x, rect.y + rect.height - amount), new Vector2(rect.width, amount));
					return rect.Inset (0, 0, 0, amount);
			}
			inset = new Rect ();
			return rect;
		}

		public static Rect Centered (this Rect parent, float? width, float? height) {
			if (!width.HasValue && !height.HasValue) {
				return parent;
			}

			float w = width.HasValue ? width.Value : parent.width;
			float h = height.HasValue ? height.Value : parent.height;

			float wd = parent.width - w;
			float hd = parent.height - h;

			parent.x += wd * 0.5f;
			parent.y += hd * 0.5f;
			parent.width = w;
			parent.height = h;
			return parent;
		}

		public static Rect CenteredByHeight (this Rect parent, float height) {
			float offset = (parent.height - height) * 0.5f;
			return parent.Inset (offset);
		}

		public static void SplitNonAlloc (this Rect original, RectTransform.Axis axis, Rect[] parts) {
			if (parts == null || parts.Length == 0 || original == null) {
				return;
			}
			switch (axis) {
				case RectTransform.Axis.Horizontal:
					SplitHorizontal (original, parts);
					return;
				case RectTransform.Axis.Vertical:
					SplitVertical (original, parts);
					break;
			}
		}

		private static void SplitHorizontal (Rect rect, Rect[] parts) {
			var width = rect.width / parts.Length;
			var pos = 0f;
			for (int i = 0; i < parts.Length; i++) {
				parts[i] = new Rect (rect.position + Vector2.right * pos, new Vector2 (width, rect.height));
				pos += width;
			}
		}

		private static void SplitVertical (Rect rect, Rect[] parts) {
			var height = rect.height / parts.Length;
			var pos = 0f;
			for (int i = 0; i < parts.Length; i++) {
				parts[i] = new Rect (rect.position + Vector2.up * pos, new Vector2 (rect.width, height));
				pos += height;
			}
		}

	}
}