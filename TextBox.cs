using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public class TextBox : Control
	{
		public TextBox()
			: base()
		{
			Padding = new Padding(10);
			Font = new Font(Font.FontFamily, 1.5f * Font.Size);

			BackColor = ColorTheme.DefaultTheme.BackColor;
			ForeColor = ColorTheme.DefaultTheme.TextColor;

			SelectionBackColor = ColorTheme.ContrastTheme.BackColor;
			SelectionForeColor = ColorTheme.ContrastTheme.TextColor;

			BorderColor = ColorTheme.DefaultTheme.BorderColor;
			BorderWidth = 2;

			SetStyle(ControlStyles.ResizeRedraw, true);

			blinkCaretTimer.Tick += (s, e) => IsCaretVisible = !IsCaretVisible;
			keyTimers[Keys.Left  ].Tick += (s, e) => moveCaretLeft();
			keyTimers[Keys.Right ].Tick += (s, e) => moveCaretRight();
			keyTimers[Keys.Back  ].Tick += (s, e) => backspace();
			keyTimers[Keys.Delete].Tick += (s, e) => delete();
		}

		#region sizing
		public override bool AutoSize {
			get { return true; }
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size textSize = TextRenderer.MeasureText(Text, Font);
			return new Size(
				Math.Max(proposedSize.Width, textSize.Width + Padding.Horizontal),
				Math.Max(proposedSize.Height, textSize.Height + Padding.Vertical)
			);
		}

		public override Size MinimumSize {
			get { return new Size(0, Font.Height + Padding.Vertical); }
		}
		#endregion

		#region styling
		protected override Cursor DefaultCursor {
			get { return Cursors.IBeam; }
		}

		public Color SelectionBackColor {
			get;
			set;
		}

		public Color SelectionForeColor {
			get;
			set;
		}

		public Color BorderColor {
			get;
			set;
		}

		public int BorderWidth {
			get;
			set;
		}
		#endregion

		#region CaretPosition
		private int caretPosition = 0;

		public int CaretPosition {
			get { return caretPosition; }
			set {
				if (caretPosition != value) {
					if (Text.Length < value || value < 0)
						throw new ArgumentOutOfRangeException();
					caretPosition = value;

					if (CaretSelectionEnabled) {
						if (value > selectionOrigin) {
							SelectionStart = selectionOrigin;
							SelectionLength = value - selectionOrigin;
						} else {
							SelectionLength = selectionOrigin - value;
							SelectionStart = value;
						}
					} else
						SelectionLength = 0;

					Invalidate();
				}
			}
		}

		protected virtual int GetCaretPosition(int x)
		{
			float lastDelta = Math.Abs(charPositions[0] - x);
			for (int i = 1; i < Text.Length; ++i) {
				float delta = Math.Abs(charPositions[i] - x);
				if (lastDelta < delta)
					return i - 1;
				lastDelta = delta;
			}
			return Text.Length;
		}
		#endregion

		#region IsCaretVisible
		private bool caretVisible = false;

		protected bool IsCaretVisible {
			get { return caretVisible; }
			set {
				if (caretVisible != value) {
					caretVisible = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region Editable
		private bool editable = true;

		public bool Editable {
			get { return editable; }
			set {
				if (editable != value) {
					editable = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region Selection
		private int selectionOrigin;

		private int selectionStart = 0;

		public int SelectionStart {
			get { return selectionStart; }
			set {
				if (selectionStart != value) {
					selectionStart = value;
					Invalidate();
				}
			}
		}

		private int selectionLength = 0;

		public int SelectionLength {
			get { return selectionLength; }
			set {
				if (selectionLength != value) {
					selectionLength = value;
					Invalidate();
				}
			}
		}

		private bool CaretSelectionEnabled {
			get { return mouseSelection || keySelection; }
		}

		private bool mouseSelection = false;
		private bool keySelection   = false;
		#endregion

		#region focus handling
		private readonly Timer blinkCaretTimer = new Timer() { Interval = 500 };

		protected override void OnGotFocus(EventArgs e)
		{
			blinkCaretTimer.Start();
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			blinkCaretTimer.Stop();
			IsCaretVisible = false;
			base.OnLostFocus(e);
		}
		#endregion

		#region mouse handling
		protected override void OnMouseDown(MouseEventArgs e)
		{
			selectionOrigin = SelectionStart = CaretPosition = GetCaretPosition(e.Location.X);
			SelectionLength = 0;
			mouseSelection = true;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseSelection = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mouseSelection)
				CaretPosition = GetCaretPosition(e.Location.X);
			base.OnMouseMove(e);
		}
		#endregion

		#region key handling
		private readonly Dictionary<Keys, TickOnceTimer> keyTimers = new Dictionary<Keys, TickOnceTimer>() {
			{ Keys.Left,   new TickOnceTimer(500) },
			{ Keys.Right,  new TickOnceTimer(500) },
			{ Keys.Back,   new TickOnceTimer(500) },
			{ Keys.Delete, new TickOnceTimer(500) }
		};

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.ShiftKey:
					keySelection = true;
					selectionOrigin = CaretPosition;
					break;
				case Keys.Delete:
				case Keys.Back:
					keyTimers[e.KeyCode].EnsureOneTick = (SelectionLength == 0);
					if (SelectionLength > 0)
						DeleteSelection();
					break;
			}

			if (keyTimers.ContainsKey(e.KeyCode))
				keyTimers[e.KeyCode].Start();

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.ShiftKey:
					keySelection = false;
					break;
				case Keys.Delete:
				case Keys.Back:
					keyTimers[e.KeyCode].EnsureOneTick = (SelectionLength == 0);
					break;
			}

			if (keyTimers.ContainsKey(e.KeyCode))
				keyTimers[e.KeyCode].Stop();

			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			return base.IsInputKey(keyData)
				|| keyData == Keys.Left || keyData == Keys.Right
				|| keyData == (Keys.Shift | Keys.Left) || keyData == (Keys.Shift | Keys.Right);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (Editable && !char.IsControl(e.KeyChar)) {
				Text = Text.Insert(CaretPosition, e.KeyChar.ToString());
				CaretPosition++;
			}
			base.OnKeyPress(e);
		}

		private void moveCaretLeft()
		{
			CaretPosition = Math.Max(CaretPosition - 1, 0);
		}

		private void moveCaretRight()
		{
			CaretPosition = Math.Min(CaretPosition + 1, Text.Length);
		}

		private void backspace()
		{
			if (Editable && CaretPosition > 0) {
				Text = Text.Substring(0, CaretPosition - 1) + Text.Substring(CaretPosition);
				if (CaretPosition < Text.Length) // if it is >= the length, it is adjusted automatically by OnTextChanged()
					CaretPosition--;
			}
		}

		private void delete()
		{
			if (Editable && CaretPosition < Text.Length)
				Text = Text.Substring(0, CaretPosition) + Text.Substring(CaretPosition + 1);
		}
		#endregion

		public void DeleteSelection()
		{
			Text = Text.Substring(0, SelectionStart) + Text.Substring(SelectionStart + SelectionLength);
			SelectionLength = 0;
			CaretPosition = SelectionStart;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if (CaretPosition > Text.Length)
				CaretPosition = Text.Length;
			base.OnTextChanged(e);
		}

		#region painting
		protected int[] charPositions;

		protected override void OnPaint(PaintEventArgs e)
		{
			using (Pen borderPen = new Pen(BorderColor, BorderWidth))
				e.Graphics.DrawRectangle(borderPen, GeometryHelper.ApplyPadding(ClientRectangle, new Padding(BorderWidth / 2)));

			DrawAndMeasureText(e.Graphics);

			if (caretVisible)
				DrawCaret(e.Graphics);

			if (!Enabled)
				PaintHelper.DrawDisabledFilter(e.Graphics, this);
		}

		protected virtual void DrawAndMeasureText(Graphics g)
		{
			Rectangle textRect = GeometryHelper.ApplyPadding(ClientRectangle, Padding);
			TextFormatFlags flags =
				TextFormatFlags.Left
				| TextFormatFlags.VerticalCenter
				| TextFormatFlags.SingleLine
				| TextFormatFlags.TextBoxControl
				| TextFormatFlags.NoPadding;

			charPositions = new int[Text.Length+1];
			charPositions[0] = Padding.Left;

			int currentTotalWidth = Padding.Left;
			for (int i = 0; i < Text.Length; ++i) {
				int charWidth = TextRenderer.MeasureText(g, Text[i].ToString(), Font, textRect.Size, flags).Width;

				bool isSelected = (SelectionStart <= i && i < SelectionStart + SelectionLength);
				if (isSelected)
					using (Brush brush = new SolidBrush(SelectionBackColor))
						g.FillRectangle(brush, new Rectangle(textRect.Left, textRect.Top, charWidth, textRect.Height));

				TextRenderer.DrawText(g, Text[i].ToString(), Font, textRect, isSelected ? SelectionForeColor : ForeColor, flags);

				charPositions[i+1] = currentTotalWidth + charWidth;
				currentTotalWidth += charWidth;
				textRect = new Rectangle(textRect.Left + charWidth, textRect.Top, textRect.Width - charWidth, textRect.Height);
			}
		}

		protected virtual void DrawCaret(Graphics g)
		{
			using (Brush caretBrush = new SolidBrush(Color.Black))
				g.FillRectangle(
					caretBrush,
					new Rectangle(
						charPositions[CaretPosition],
						Padding.Top,
						2,
						ClientRectangle.Height - Padding.Vertical
					)
				);
		}
		#endregion
	}
}