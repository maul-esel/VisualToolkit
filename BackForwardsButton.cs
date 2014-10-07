using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VisualToolkit
{
	public class BackForwardsButton : ButtonBase
	{
		public BackForwardsButton(ButtonDirection direction)
			: base()
		{
			Direction = direction;

			ForeColor = ColorTheme.DefaultTheme.SymbolColor;
			PressedForeColor = ColorTheme.HighlightTheme.SymbolColor;
		}

		protected override Size DefaultSize {
			get {
				return new Size(50, 50);
			}
		}

		#region Direction
		public enum ButtonDirection {
			Back,
			Forwards
		}

		private ButtonDirection direction;

		public ButtonDirection Direction {
			get { return direction; }
			set {
				if (direction != value) {
					direction = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region painting
		private const int symbolWidth = 4;

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

			RectangleF buttonRectangle = GetButtonRectangle();

			using (Pen borderPen = new Pen(IsPressed ? PressedBorderColor : BorderColor, BorderWidth))
				e.Graphics.DrawEllipse(borderPen, buttonRectangle);

			using (Pen symbolPen = new Pen(IsPressed ? PressedForeColor : ForeColor, symbolWidth))
				e.Graphics.DrawLines(symbolPen, GetSymbol(buttonRectangle));

			base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			using (Brush brush = new SolidBrush(IsPressed ? PressedBackColor : BackColor))
				e.Graphics.FillEllipse(brush, GetButtonRectangle());
		}

		protected virtual RectangleF GetButtonRectangle()
		{
			int size = Math.Min(ClientRectangle.Width, ClientRectangle.Height) - 2 * BorderWidth;
			RectangleF rect = new RectangleF(
				(ClientRectangle.Width - size) / 2f,
				(ClientRectangle.Height - size) / 2f,
				size,
				size
			);
			if (IsPressed)
				rect = GeometryHelper.ApplyPadding(rect, PressExpansion);
			return rect;
		}

		protected PointF[] GetSymbol(RectangleF buttonRectangle)
		{
			if (Direction == ButtonDirection.Forwards)
				return new[] {
					new PointF(buttonRectangle.Left + buttonRectangle.Width / 3f,     buttonRectangle.Top + buttonRectangle.Height / 4f),
					new PointF(buttonRectangle.Left + 2 * buttonRectangle.Width / 3f, buttonRectangle.Top + buttonRectangle.Height / 2f),
					new PointF(buttonRectangle.Left + buttonRectangle.Width / 3f,     buttonRectangle.Top + 3 * buttonRectangle.Height / 4f)
				};
			else
				return new[] {
					new PointF(buttonRectangle.Left + 2 * buttonRectangle.Width / 3f, buttonRectangle.Top + buttonRectangle.Height / 4f),
					new PointF(buttonRectangle.Left + buttonRectangle.Width / 3f,     buttonRectangle.Top + buttonRectangle.Height / 2f),
					new PointF(buttonRectangle.Left + 2 * buttonRectangle.Width / 3f, buttonRectangle.Top + 3 * buttonRectangle.Height / 4f)
				};
		}
		#endregion
	}
}