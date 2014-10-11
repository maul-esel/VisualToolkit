using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public abstract class ScrollableControl : Control
	{
		private readonly Scrollbar hscroll = new Scrollbar(Orientation.Horizontal) { Dock = DockStyle.Bottom };
		private readonly Scrollbar vscroll = new Scrollbar(Orientation.Vertical) { Dock = DockStyle.Right };

		public ScrollableControl()
		{
			Controls.Add(hscroll);
			Controls.Add(vscroll);

			hscroll.Scroll += (s, e) => Invalidate();
			vscroll.Scroll += (s, e) => Invalidate();
		}

		public Point ScrollPosition {
			get {
				return new Point(
					(int)((ContentRectangle.Width  / (float)ScrollSize.Width  - 1) * hscroll.ScrollValue),
					(int)((ContentRectangle.Height / (float)ScrollSize.Height - 1) * vscroll.ScrollValue)
				);
			}
			set {
				hscroll.ScrollValue = (int)(value.X / (1 - ContentRectangle.Width  / (float)ScrollSize.Width));
				vscroll.ScrollValue = (int)(value.Y / (1 - ContentRectangle.Height / (float)ScrollSize.Height));
			}
		}

		protected Rectangle ContentRectangle {
			get {
				return new Rectangle(
					ClientRectangle.Location,
					new Size(
						ClientRectangle.Width -  (vscroll.Visible ? vscroll.Thickness : 0),
						ClientRectangle.Height - (hscroll.Visible ? hscroll.Thickness : 0)
					)
				);
			}
		}

		#region ScrollSize
		private Size scrollSize;

		protected virtual Size ScrollSize {
			get { return scrollSize; }
			set {
				if (scrollSize != value) {
					scrollSize = value;
					OnScrollSizeChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler ScrollSizeChanged;

		protected virtual void OnScrollSizeChanged(EventArgs e)
		{
			hscroll.MaximumValue = ScrollSize.Width;
			vscroll.MaximumValue = ScrollSize.Height;

			adjustScrollbars();

			EventHandler handler = ScrollSizeChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		protected override void OnSizeChanged(EventArgs e)
		{
			adjustScrollbars();
			base.OnSizeChanged(e);
		}

		private void adjustScrollbars()
		{
			hscroll.Visible = (ScrollSize.Width > ClientSize.Width);
			if (!hscroll.Visible)
				hscroll.ScrollValue = 0;

			vscroll.Visible = (ScrollSize.Height > ClientSize.Height);
			if (!vscroll.Visible)
				vscroll.ScrollValue = 0;
		}

		protected virtual void ScrollIntoView(Rectangle rect)
		{
			if (!ContentRectangle.Contains(rect)) {
				int x = -ScrollPosition.X, y = -ScrollPosition.Y;

				if (ContentRectangle.Bottom < rect.Bottom)
					y += rect.Bottom - ContentRectangle.Bottom;
				if (ContentRectangle.Top > rect.Top)
					y -= ContentRectangle.Top - rect.Top;

				if (ContentRectangle.Right < rect.Right)
					x += rect.Right - ContentRectangle.Right;
				if (ContentRectangle.Left > rect.Left)
					x -= ContentRectangle.Left - rect.Left;

				ScrollPosition = new Point(x, y);
			}
		}
	}
}