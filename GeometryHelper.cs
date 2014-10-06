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
	}
}