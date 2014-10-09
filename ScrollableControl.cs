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

		#region ScrollSize
		private Size scrollSize;

		public virtual Size ScrollSize {
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
	}
}