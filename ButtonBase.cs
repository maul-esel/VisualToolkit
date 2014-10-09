using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public abstract class ButtonBase : Control, IButtonControl
	{
		public ButtonBase()
			: base()
		{
			BorderWidth = 2;
			Padding = new Padding(1);

			BackColor = ColorTheme.DefaultTheme.BackColor;
			BorderColor = ColorTheme.DefaultTheme.BorderColor;

			PressedBackColor = ColorTheme.HighlightTheme.BackColor;
			PressedBorderColor = ColorTheme.HighlightTheme.BorderColor;

			SetStyle(ControlStyles.StandardClick, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected virtual Padding PressExpansion {
			get {
				return new Padding(1);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!Enabled)
				PaintHelper.DrawDisabledFilter(e.Graphics, this);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			PaintHelper.FillWithParentBackground(e.Graphics, this);
		}

		public int BorderWidth {
			get;
			set;
		}

		#region colors
		public Color BorderColor {
			get;
			set;
		}

		public Color PressedBorderColor {
			get;
			set;
		}

		public Color PressedBackColor {
			get;
			set;
		}

		public Color PressedForeColor {
			get;
			set;
		}
		#endregion

		#region IsPressed
		private bool isPressed = false;

		protected bool IsPressed {
			get { return isPressed; }
			set {
				if (value != isPressed) {
					isPressed = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region mouse and key handling
		protected override void OnMouseDown(MouseEventArgs e)
		{
			IsPressed = (e.Button == MouseButtons.Left);
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!ClientRectangle.Contains(e.Location))
				IsPressed = false;
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (IsPressed) {
				IsPressed = false;
				PerformClick();
			}
			base.OnMouseUp(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
				IsPressed = true;
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
				IsPressed = false;
				PerformClick();
			}
			base.OnKeyUp(e);
		}
		#endregion

		#region IButtonControl
		public DialogResult DialogResult {
			get;
			set;
		}

		public virtual void NotifyDefault(bool value)
		{
		}

		public virtual void PerformClick()
		{
			OnClick(EventArgs.Empty);
		}
		#endregion
	}
}

