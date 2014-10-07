using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public class Button : ButtonBase
	{
		public Button()
			: base()
		{
			Font = new Font(Font.FontFamily, 1.25f * Font.Size);

			ForeColor = ColorTheme.DefaultTheme.TextColor;
			PressedForeColor = ColorTheme.HighlightTheme.TextColor;
		}

		protected override Size DefaultSize {
			get { return new Size(100, 40); }
		}

		protected override void OnTextChanged(EventArgs e)
		{
			Size minSize = TextRenderer.MeasureText(Text, Font);
			Size = new Size(
				Math.Max(minSize.Width + Padding.Horizontal, Size.Width),
				Math.Max(minSize.Height + Padding.Vertical,  Size.Height)
			);
			base.OnTextChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle rect = GeometryHelper.ApplyPadding(ClientRectangle, Padding);
			if (IsPressed)
				rect = GeometryHelper.ApplyPadding(rect, PressExpansion);

			StringFormat format = new StringFormat() {
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			using (Pen borderPen = new Pen(IsPressed ? PressedBorderColor : BorderColor, BorderWidth))
				e.Graphics.DrawRectangle(borderPen, rect);

			using (Brush fgBrush = new SolidBrush(IsPressed ? PressedForeColor : ForeColor))
				e.Graphics.DrawString(Text, Font, fgBrush, rect, format);

			base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			using (Brush brush = new SolidBrush(IsPressed ? PressedBackColor : BackColor))
				e.Graphics.FillRectangle(brush, IsPressed ? ClientRectangle : GeometryHelper.ApplyPadding(ClientRectangle, PressExpansion));
		}
	}
}