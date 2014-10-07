using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public class Scrollbar : Control
	{
		public Scrollbar(Orientation orientation)
		{
			this.orientation = orientation;
			Thickness = 15;

			BackColor = ColorTheme.DefaultTheme.BackColor;
			ForeColor = ColorTheme.DefaultTheme.SymbolColor;
			ButtonBackColor = ColorTheme.ContrastTheme.BackColor;
			ButtonSymbolColor = ColorTheme.ContrastTheme.SymbolColor;

			moveNearTimer.Tick += (s, e) => moveNear();
			moveFarTimer.Tick += (s, e) => moveFar();

			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		public Color ButtonBackColor {
			get;
			set;
		}

		public Color ButtonSymbolColor {
			get;
			set;
		}

		#region Thickness
		private int thickness;

		public int Thickness {
			get { return thickness; }
			set {
				if (thickness != value) {
					thickness = value;
					switch (Orientation) {
						case Orientation.Horizontal:
							Height = value;
							break;
						case Orientation.Vertical:
							Width = value;
							break;
					}
				}
			}
		}
		#endregion

		#region Orientation
		private readonly Orientation orientation;

		public Orientation Orientation {
			get { return orientation; }
		}
		#endregion

		#region MinimumValue
		private int minimumValue = 0;

		public int MinimumValue {
			get { return minimumValue; }
			set {
				if (minimumValue != value) {
					int oldDistance = ScrollDistance;
					minimumValue = value;
					scaleScrollValue(oldDistance);
				}
			}
		}
		#endregion

		#region MaximumValue
		private int maximumValue = 100;

		public int MaximumValue {
			get { return maximumValue; }
			set {
				if (maximumValue != value) {
					int oldDistance = ScrollDistance;
					maximumValue = value;
					scaleScrollValue(oldDistance);
				}
			}
		}
		#endregion

		private void scaleScrollValue(int oldDistance)
		{
			ScrollValue = (int)(ScrollValue / (float)oldDistance * ScrollDistance + MinimumValue);
		}

		#region Scrolling
		private int scrollValue = 0;

		public int ScrollValue {
			get { return scrollValue; }
			set {
				if (scrollValue != value) {
					scrollValue = value;
					OnScroll(new EventArgs());
				}
			}
		}

		public event EventHandler Scroll;

		protected virtual void OnScroll(EventArgs e)
		{
			Invalidate();
	
			EventHandler handler = Scroll;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		public int ScrollDistance {
			get { return MaximumValue - MinimumValue; }
		}

		protected virtual Size ButtonSize {
			get { return new Size(Thickness, Thickness); }
		}

		#region mouse handling
		private bool isDragging = false;
		private Point lastDraggingPoint;

		private readonly Timer moveNearTimer = new Timer() { Interval = 40 };
		private readonly Timer moveFarTimer = new Timer() { Interval = 40 };

		private DateTime moveTimerStarted;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				if (GetScrollThumbRectangle().Contains(e.Location)) {
					isDragging = true;
					lastDraggingPoint = e.Location;
				} else if (GetNearButtonRectangle().Contains(e.Location)) {
					moveNearTimer.Start();
					moveTimerStarted = DateTime.UtcNow;
				} else if (GetFarButtonRectangle().Contains(e.Location)) {
					moveFarTimer.Start();
					moveTimerStarted = DateTime.Now;
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (isDragging) {
				int pixelDistance = GeometryHelper.GetDimension(e.Location, Orientation) - GeometryHelper.GetDimension(lastDraggingPoint, Orientation);
				int areaLength = GeometryHelper.GetDimension(ClientSize, Orientation) - 2 * GeometryHelper.GetDimension(ButtonSize, Orientation);
				int scrollDelta = (int)(pixelDistance / (float)areaLength * ScrollDistance);

				ScrollValue = Math.Min(Math.Max(ScrollValue + scrollDelta, MinimumValue), MaximumValue);
				lastDraggingPoint = e.Location;
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			isDragging = false;

			if (moveNearTimer.Enabled) {
				moveNearTimer.Stop();
				if ((DateTime.UtcNow - moveTimerStarted).TotalMilliseconds < moveNearTimer.Interval)
					moveNear();
			}

			if (moveFarTimer.Enabled) {
				moveFarTimer.Stop();
				if ((DateTime.UtcNow - moveTimerStarted).TotalMilliseconds < moveFarTimer.Interval)
					moveFar();
			}

			base.OnMouseUp(e);
		}

		private const int scrollDelta = 15;

		private void moveNear()
		{
			ScrollValue = Math.Max(ScrollValue - scrollDelta, MinimumValue);
		}

		private void moveFar()
		{
			ScrollValue = Math.Min(ScrollValue + scrollDelta, MaximumValue);
		}
		#endregion

		#region painting
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			using (Brush brush = new SolidBrush(ForeColor))
				e.Graphics.FillRectangle(brush, GetScrollThumbRectangle());

			using (Brush bgBrush = new SolidBrush(ButtonBackColor)) {
				e.Graphics.FillRectangle(bgBrush, GetNearButtonRectangle());
				e.Graphics.FillRectangle(bgBrush, GetFarButtonRectangle());
			}

			using (Brush fgBrush = new SolidBrush(ButtonSymbolColor)) {
				e.Graphics.FillPolygon(fgBrush, GetNearButtonSymbol());
				e.Graphics.FillPolygon(fgBrush, GetFarButtonSymbol());
			}
		}

		protected virtual Rectangle GetNearButtonRectangle()
		{
			return new Rectangle(new Point(0, 0), ButtonSize);
		}

		protected virtual Rectangle GetFarButtonRectangle()
		{
			switch (Orientation) {
				case Orientation.Horizontal:
					return new Rectangle(new Point(ClientSize.Width - Thickness, 0), ButtonSize);
				case Orientation.Vertical:
					return new Rectangle(new Point(0, ClientSize.Height - Thickness), ButtonSize);
			}
			throw new NotSupportedException();
		}

		protected virtual PointF[] GetNearButtonSymbol()
		{
			switch (Orientation) {
				case Orientation.Horizontal:
					return new PointF[] {
						new PointF(ButtonSize.Width / 4f, ButtonSize.Height / 2f),
						new PointF(3 * ButtonSize.Width / 4f, ButtonSize.Height / 4f),
						new PointF(3 * ButtonSize.Width / 4f, 3 * ButtonSize.Height / 4f)
					};
				case Orientation.Vertical:
					return new PointF[] {
						new PointF(ButtonSize.Width / 2f, ButtonSize.Height / 4f),
						new PointF(ButtonSize.Width / 4f, 3 * ButtonSize.Height / 4f),
						new PointF(3 * ButtonSize.Width / 4f, 3 * ButtonSize.Height / 4f)
					};
			}
			throw new NotSupportedException();
		}

		protected virtual PointF[] GetFarButtonSymbol()
		{
			PointF[] symbol = GetNearButtonSymbol();
			switch (Orientation) {
				case Orientation.Horizontal:
					for (int i = 0; i < symbol.Length; ++i)
						symbol[i].X = ClientSize.Width - symbol[i].X;
					break;
				case Orientation.Vertical:
					for (int i = 0; i < symbol.Length; ++i)
						symbol[i].Y = ClientSize.Height - symbol[i].Y;
					break;
			}
			return symbol;
		}

		protected virtual RectangleF GetScrollThumbRectangle()
		{
			int areaLength = GeometryHelper.GetDimension(ClientRectangle, Orientation) - 2 * GeometryHelper.GetDimension(ButtonSize, Orientation);
			float thumbLength = areaLength * areaLength / (float)ScrollDistance;
			float offset = ScrollValue / (float)ScrollDistance * (areaLength - thumbLength) + GeometryHelper.GetDimension(ButtonSize, Orientation);

			switch (Orientation) {
				case Orientation.Horizontal:
					return new RectangleF(
						ClientRectangle.Left + offset,
						ClientRectangle.Top,
						thumbLength,
						Thickness
					);
				case Orientation.Vertical:
					return new RectangleF(
						ClientRectangle.Left,
						ClientRectangle.Top + offset,
						Thickness,
						thumbLength
					);
			}
			throw new NotSupportedException();
		}
		#endregion
	}
}