using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	internal static class PaintHelper
	{
		internal static void DrawDisabledFilter(Graphics g, Rectangle rect)
		{
			using (Brush brush = new SolidBrush(Color.FromArgb(75, Color.White)))
				g.FillRectangle(brush, rect);
		}

		internal static void DrawDisabledFilter(Graphics g, Control ctrl)
		{
			DrawDisabledFilter(g, ctrl.ClientRectangle);
		}
	}
}