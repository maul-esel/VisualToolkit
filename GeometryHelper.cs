using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public static class GeometryHelper
	{
		public static int GetDimension(Rectangle rect, Orientation orientation)
		{
			return GetDimension(rect.Size, orientation);
		}

		public static int GetDimension(Size size, Orientation orientation)
		{
			switch (orientation) {
				case Orientation.Horizontal:
					return size.Width;
				case Orientation.Vertical:
					return size.Height;
			}
			throw new NotSupportedException();
		}

		public static int GetDimension(Point pt, Orientation orientation)
		{
			switch (orientation) {
				case Orientation.Horizontal:
					return pt.X;
				case Orientation.Vertical:
					return pt.Y;
			}
			throw new NotSupportedException();
		}

		public static Rectangle ApplyPadding(Rectangle rect, Padding pad)
		{
			return new Rectangle(
				rect.Left + pad.Left,
				rect.Top + pad.Top,
				rect.Width - pad.Horizontal,
				rect.Height - pad.Vertical
			);
		}

		public static RectangleF ApplyPadding(RectangleF rect, Padding pad)
		{
			return new RectangleF(
				rect.Left + pad.Left,
				rect.Top + pad.Top,
				rect.Width - pad.Horizontal,
				rect.Height - pad.Vertical
			);
		}
	}
}