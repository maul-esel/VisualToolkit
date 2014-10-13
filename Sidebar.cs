using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VisualToolkit
{
	public partial class Sidebar : Control
	{
		public Sidebar()
		{
			ItemHeight = 75;
			BorderWidth = 1;
			SymbolWidth = 5;
			ItemPadding = new Padding(10);

			Font = new Font(Font.FontFamily, 1.75f * Font.Size);

			ForeColor   = ColorTheme.DefaultTheme.TextColor;
			SymbolColor = ColorTheme.DefaultTheme.SymbolColor;
			BorderColor = ColorTheme.DefaultTheme.BorderColor;
			BackColor   = ColorTheme.DefaultTheme.BackColor;

			HoveredItemForeColor   = ColorTheme.ContrastTheme.TextColor;
			HoveredItemSymbolColor = ColorTheme.ContrastTheme.SymbolColor;
			HoveredItemBorderColor = ColorTheme.ContrastTheme.BorderColor;
			HoveredItemBackColor   = ColorTheme.ContrastTheme.BackColor;

			items.CollectionChanged += OnItemsChanged;
		}

		public Sidebar(IEnumerable<Item> items)
			: this()
		{
			Items.AddRange(items);
		}

		#region styling
		public int ItemHeight {
			get;
			set;
		}

		public int BorderWidth {
			get;
			set;
		}

		public int SymbolWidth {
			get;
			set;
		}

		public Padding ItemPadding {
			get;
			set;
		}

		public Color SymbolColor {
			get;
			set;
		}

		public Color BorderColor {
			get;
			set;
		}

		public Color HoveredItemBackColor {
			get;
			set;
		}

		public Color HoveredItemForeColor {
			get;
			set;
		}

		public Color HoveredItemSymbolColor {
			get;
			set;
		}

		public Color HoveredItemBorderColor {
			get;
			set;
		}
		#endregion

		#region Items
		private EventableCollection<Item> items = new EventableCollection<Item>(false);

		public EventableCollection<Item> Items {
			get { return items; }
			set {
				if (value != items) {
					if (items != null)
						items.CollectionChanged -= OnItemsChanged;
					items = value;
					if (value != null)
						value.CollectionChanged += OnItemsChanged;
					OnItemsChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler ItemsChanged;

		private void OnItemsChanged(object sender, EventArgs e)
		{
			OnItemsChanged(e);
		}

		protected virtual void OnItemsChanged(EventArgs e)
		{
			Invalidate();

			EventHandler handler = ItemsChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region ItemInvoked
		protected virtual void InvokeItem(Item item)
		{
			if (item != null) {
				item.RaiseInvoked();
				OnItemInvoked(new ItemEventArgs<Item>(item));
			}
		}

		public event ItemEventHandler<Item> ItemInvoked;

		protected virtual void OnItemInvoked(ItemEventArgs<Item> e)
		{
			ItemEventHandler<Item> handler = ItemInvoked;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region HoveredItem
		private Item hoveredItem = null;

		protected Item HoveredItem {
			get { return hoveredItem; }
			set {
				if (hoveredItem != value) {
					hoveredItem = value;
					Invalidate();
				}
			}
		}
		#endregion

		protected override void OnMouseMove(MouseEventArgs e)
		{
			HoveredItem = HitTest(e.Location);
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			HoveredItem = null;
			base.OnMouseLeave(e);
		}

		public virtual Item HitTest(Point pt)
		{
			for (int i = 0; i < Items.Count; ++i)
				if (GetItemBounds(i).Contains(pt))
					return Items[i];
			return null;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			InvokeItem(HoveredItem);
		}

		#region painting
		protected override void OnPaint(PaintEventArgs e)
		{
			for (int i = 0; i < Items.Count; ++i)
				DrawItem(e.Graphics, Items[i], GetItemBounds(i));
		}

		protected virtual void DrawItem(Graphics g, Item item, Rectangle bounds)
		{
			g.SmoothingMode = SmoothingMode.HighQuality;

			bool isHovered = (item == HoveredItem);
			using (Brush bgBrush = new SolidBrush(isHovered ? HoveredItemBackColor : BackColor))
				g.FillRectangle(bgBrush, bounds);

			using (Pen borderPen = new Pen(isHovered ? HoveredItemBorderColor : BorderColor, BorderWidth)) {
				g.DrawLine(borderPen, bounds.Location, new Point(bounds.Right, bounds.Top));
				g.DrawLine(borderPen, new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Bottom));
			}

			StringFormat format = new StringFormat() {
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center
			};
			using (Brush textBrush = new SolidBrush(isHovered ? HoveredItemForeColor : ForeColor))
				g.DrawString(
					item.Text,
					Font,
					textBrush,
					new RectangleF(
						bounds.Left + ItemPadding.Left,
						bounds.Top + ItemPadding.Top,
						0.70f * (bounds.Width - ItemPadding.Horizontal),
						bounds.Height - ItemPadding.Vertical
					),
					format
				);

			using (Pen symbolPen = new Pen(isHovered ? HoveredItemSymbolColor : SymbolColor, SymbolWidth))
				g.DrawLines(symbolPen, GetSymbol(bounds));
		}

		protected virtual Rectangle GetItemBounds(int index)
		{
			return new Rectangle(
				ClientRectangle.Left + Padding.Left,
				ClientRectangle.Top + Padding.Top + index * ItemHeight,
				ClientRectangle.Width - Padding.Horizontal,
				ItemHeight
			);
		}

		protected virtual PointF[] GetSymbol(Rectangle bounds)
		{
			float x = bounds.Left + ItemPadding.Left + 0.75f * (bounds.Width - ItemPadding.Horizontal);
			float maxWidth = bounds.Right - ItemPadding.Right - x;
			float maxHeight = (bounds.Height - Padding.Vertical) / 3f;

			float size = Math.Min(maxWidth, maxHeight);

			return new[] {
				new PointF(x, bounds.Top + (bounds.Height - size) / 2f),
				new PointF(x + 2 * size/3f, bounds.Top + bounds.Height / 2f),
				new PointF(x, bounds.Top + (bounds.Height - size) / 2f + size)
			};
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			PaintHelper.FillWithParentBackground(e.Graphics, this);
		}
		#endregion
	}
}