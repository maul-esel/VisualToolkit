using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	internal static class PaintHelper
	{
		internal static readonly Brush DisabledFilter = new SolidBrush(Color.FromArgb(75, Color.White));
		internal static void DrawDisabledFilter(Graphics g, Rectangle rect)
		{
			g.FillRectangle(DisabledFilter, rect);
		}

		internal static void DrawDisabledFilter(Graphics g, Control ctrl)
		{
			DrawDisabledFilter(g, ctrl.ClientRectangle);
		}

		internal static void FillWithParentBackground(Graphics g, Control ctrl)
		{
			Color parentBack = ctrl.Parent.BackColor;
			using (Brush brush = new SolidBrush(parentBack))
				g.FillRectangle(brush, ctrl.ClientRectangle);
		}
	}
}