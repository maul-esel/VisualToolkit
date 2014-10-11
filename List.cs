using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace VisualToolkit
{
	public partial class List : ScrollableControl
	{
		public List()
			: base()
		{
			Font = new Font(Font.FontFamily, 1.25f * Font.Size);
			TitleFont = new Font(Font.FontFamily, 1.25f * Font.Size);

			itemList.CollectionChanged += OnItemsChanged;
			groupList.CollectionChanged += OnGroupsChanged;

			ItemHeight = 75;
			ItemBorderWidth = 2;

			ForeColor       = ColorTheme.DefaultTheme.TextColor;
			ItemBackColor   = ColorTheme.DefaultTheme.BackColor;
			ItemBorderColor = ColorTheme.DefaultTheme.BorderColor;

			SelectedItemForeColor   = ColorTheme.DefaultTheme.TextColor;
			SelectedItemBackColor   = ColorTheme.DefaultTheme.BackColor;
			SelectedItemBorderColor = ColorTheme.DefaultTheme.BorderColor;

			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

			moveSelectionUpTimer.Tick += (s, e) => moveSelectionUp();
			moveSelectionDownTimer.Tick += (s, e) => moveSelectionDown();
		}

		public List(IEnumerable<Item> items)
			: this()
		{
			itemList.AddRange(items);
		}

		public override bool AutoSize {
			get { return true; }
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return new Size(proposedSize.Width, Math.Max(proposedSize.Height, ScrollSize.Height));
		}

		#region styling
		public Color ItemBackColor {
			get;
			set;
		}

		public Color ItemBorderColor {
			get;
			set;
		}

		public int ItemBorderWidth {
			get;
			set;
		}

		public Color SelectedItemForeColor {
			get;
			set;
		}

		public Color SelectedItemBackColor {
			get;
			set;
		}

		public Color SelectedItemBorderColor {
			get;
			set;
		}

		public int ItemHeight {
			get;
			set;
		}

		public Font TitleFont {
			get;
			set;
		}
		#endregion

		private bool itemsFromDataSource = false;

		#region Items
		private EventableCollection<Item> itemList = new EventableCollection<Item>();

		public EventableCollection<Item> Items {
			get { return itemList; }
			set {
				if (itemList != value) {
					if (itemList != null)
						itemList.CollectionChanged -= OnItemsChanged;
					itemList = value;
					if (value != null)
						value.CollectionChanged += OnItemsChanged;
					itemsFromDataSource = false;
					OnItemsChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler ItemsChanged;

		private void OnItemsChanged(object sender, EventArgs e)
		{
			itemsFromDataSource = false;
			OnItemsChanged(e);
		}

		protected virtual void OnItemsChanged(EventArgs e)
		{
			OnScrollSizeChanged(e);
			Invalidate();

			EventHandler handler = ItemsChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region Show* properties
		private bool showImages = true;

		public bool ShowImages {
			get { return showImages; }
			set {
				if (showImages != value) {
					showImages = value;
					Invalidate();
				}
			}
		}

		private bool showTitles = true;

		public bool ShowTitles {
			get { return showTitles; }
			set {
				if (showTitles != value) {
					showTitles = value;
					Invalidate();
				}
			}
		}

		private bool showDetails = true;

		public bool ShowDetails {
			get { return showDetails; }
			set {
				if (showDetails != value) {
					showDetails = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region ValueMember properties
		private string imageValueMember;

		public string ImageValueMember {
			get { return imageValueMember; }
			set {
				if (imageValueMember != value) {
					imageValueMember = value;
					ReloadDataSource();
				}
			}
		}

		private string titleValueMember;

		public string TitleValueMember {
			get { return titleValueMember; }
			set {
				if (titleValueMember != value) {
					titleValueMember = value;
					ReloadDataSource();
				}
			}
		}

		private string detailsValueMember;

		public string DetailsValueMember {
			get { return detailsValueMember; }
			set {
				if (detailsValueMember != value) {
					detailsValueMember = value;
					ReloadDataSource();
				}
			}
		}

		private string groupValueMember;

		public string GroupValueMember {
			get { return groupValueMember; }
			set {
				if (groupValueMember != value) {
					groupValueMember = value;
					ReloadDataSource();
				}
			}
		}

		private string groupTitleValueMember;

		public string GroupTitleValueMember {
			get { return groupTitleValueMember; }
			set {
				if (groupTitleValueMember != value) {
					groupTitleValueMember = value;
					ReloadDataSource();
				}
			}
		}
		#endregion

		#region DataSource
		private object dataSource;

		public object DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				LoadDataSource();
			}
		}

		protected void ReloadDataSource()
		{
			if (itemsFromDataSource)
				LoadDataSource();
		}

		private Dictionary<object, Group> groupDictionary;

		protected virtual void LoadDataSource()
		{
			if (DataSource is IEnumerable) {
				groupDictionary = new Dictionary<object, Group>();
				EventableCollection<Item> newItems = new EventableCollection<Item>();

				foreach (object item in (DataSource as IEnumerable))
					newItems.Add(new Item() {
						Title = GetProperty<string>(TitleValueMember, item),
						Details = GetProperty<string>(DetailsValueMember, item),
						Image = GetProperty<Image>(ImageValueMember, item),
						Group = getGroup(GetProperty<object>(GroupValueMember, item))
					});

				Groups = new EventableCollection<Group>(groupDictionary.Values);
				Items = newItems;
				groupDictionary = null;
			} else
				throw new NotSupportedException();

			itemsFromDataSource = true;
		}

		protected virtual T GetProperty<T>(string name, object subject)
		{
			if (subject != null && name != null) {
				PropertyInfo property = subject.GetType().GetProperty(name);
				if (property != null && typeof(T).IsAssignableFrom(property.PropertyType))
					return (T)property.GetValue(subject, null);
			}
			return default(T);
		}

		protected virtual Group getGroup(object id)
		{
			if (id == null)
				return null;
			if (!groupDictionary.ContainsKey(id))
				groupDictionary[id] = new Group() { Title = GetProperty<string>(GroupTitleValueMember, id) };
			return groupDictionary[id];
		}
		#endregion

		#region IsGrouped
		private bool isGrouped = false;

		public bool IsGrouped {
			get { return isGrouped; }
			set {
				if (isGrouped != value) {
					isGrouped = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region Groups
		private EventableCollection<Group> groupList = new EventableCollection<Group>();

		public EventableCollection<Group> Groups {
			get { return groupList; }
			set {
				if (groupList != value) {
					if (groupList != null)
						groupList.CollectionChanged -= OnGroupsChanged;
					groupList = value;
					if (value != null)
						value.CollectionChanged += OnGroupsChanged;
				}
			}
		}

		public event EventHandler GroupsChanged;

		private void OnGroupsChanged(object sender, EventArgs e)
		{
			itemsFromDataSource = false;
			OnGroupsChanged(e);
		}

		protected virtual void OnGroupsChanged(EventArgs e)
		{
			Invalidate();

			EventHandler handler = GroupsChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region SelectedItem
		private Item selectedItem = null;

		public Item SelectedItem {
			get { return selectedItem; }
			set {
				if (selectedItem != value) {
					selectedItem = value;
					OnSelectedItemChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedItemChanged;

		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (SelectedItem != null)
				ScrollIntoView(GeometryHelper.ApplyPadding(GetItemBounds(SelectedItem), new Padding(-ItemBorderWidth)));
			Invalidate();

			EventHandler handler = SelectedItemChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		public InvocationMode InvocationMode {
			get;
			set;
		}

		protected override Size ScrollSize {
			get { return new Size(ClientRectangle.Width, Padding.Vertical + Items.Count * ItemHeight); }
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			OnScrollSizeChanged(e);
			base.OnPaddingChanged(e);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			Item target = HitTest(e.Location);
			if (target != null)
				SelectedItem = target;

			base.OnMouseClick(e);
		}

		#region key handling
		private readonly TickOnceTimer moveSelectionUpTimer = new TickOnceTimer(300);
		private readonly TickOnceTimer moveSelectionDownTimer = new TickOnceTimer(300);

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Up:
					moveSelectionUpTimer.Start();
					break;
				case Keys.Down:
					moveSelectionDownTimer.Start();
					break;
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Up:
					moveSelectionUpTimer.Stop();
					break;
				case Keys.Down:
					moveSelectionDownTimer.Stop();
					break;
			}
			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			return base.IsInputKey(keyData) || keyData == Keys.Up || keyData == Keys.Down;
		}

		private void moveSelectionUp()
		{
			if (SelectedItem != null) {
				int index = Items.IndexOf(SelectedItem);
				if (index > 0)
					SelectedItem = Items[index - 1];
			}
		}

		private void moveSelectionDown()
		{
			if (SelectedItem != null) {
				int index = Items.IndexOf(SelectedItem);
				if (index < Items.Count - 1)
					SelectedItem = Items[index + 1];
			}
		}
		#endregion

		public virtual Item HitTest(Point pt)
		{
			for (int i = 0; i < Items.Count; ++i)
				if (GetItemBounds(i).Contains(pt))
					return Items[i];
			return null;
		}

		#region painting
		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle paddedClient = GeometryHelper.ApplyPadding(ContentRectangle, Padding);
			paddedClient.Offset(ScrollPosition);

			if (IsGrouped) {
				for (int i = 0; i < Groups.Count; ++i) {
					// TODO: draw header
					// TODO: draw items
				}
				throw new NotSupportedException("Grouping is not yet supported");
			} else
				for (int i = 0; i < Items.Count; ++i)
					DrawItem(Items[i], e.Graphics, GetItemBounds(i));

			base.OnPaint(e);
		}

		protected virtual Rectangle GetItemBounds(Item item)
		{
			return GetItemBounds(Items.IndexOf(item));
		}

		protected virtual Rectangle GetItemBounds(int index)
		{
			Rectangle paddedClient = GeometryHelper.ApplyPadding(ContentRectangle, Padding);
			paddedClient.Offset(ScrollPosition);

			return new Rectangle(
				paddedClient.Left,
				paddedClient.Top + index * ItemHeight,
				paddedClient.Width,
				ItemHeight
			);
		}

		protected virtual Padding SelectionExpansion {
			get { return new Padding(5); }
		}

		protected virtual void DrawGroupHeader(Group group, Graphics g, Rectangle bounds)
		{
		}

		protected virtual void DrawItem(Item item, Graphics g, Rectangle bounds)
		{
			bool isSelected = (item == SelectedItem);
			if (!isSelected)
				bounds = GeometryHelper.ApplyPadding(bounds, SelectionExpansion);

			using (Brush bgBrush = new SolidBrush(isSelected ? SelectedItemBackColor : BackColor))
				g.FillRectangle(bgBrush, bounds);
			using (Pen borderPen = new Pen(isSelected ? SelectedItemBorderColor : ItemBorderColor, ItemBorderWidth))
				g.DrawRectangle(borderPen, bounds);

			const int margin = 5;
			bounds = GeometryHelper.ApplyPadding(bounds, new Padding(margin));

			StringFormat format = new StringFormat() {
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center
			};

			int xOffset = 0;
			int yOffset = 0;
			if (ShowImages) {
				Size imageSize = new Size(bounds.Height, bounds.Height);
				if (item.Image != null)
					g.DrawImage(item.Image, new Rectangle(bounds.Location, imageSize));
				xOffset += imageSize.Width + margin;
			}

			if (ShowTitles) {
				float height = ShowDetails ? 0.25f * bounds.Height : bounds.Height;
				RectangleF textRect = new RectangleF(bounds.Left + xOffset, bounds.Top + yOffset, bounds.Width - xOffset, height);

				using (Brush brush = new SolidBrush(ForeColor))
					g.DrawString(item.Title, TitleFont, brush, textRect, format);

				yOffset += (int)height + margin;
			}

			if (ShowDetails) {
				RectangleF textRect = new RectangleF(
					bounds.Left + xOffset,
					bounds.Top + yOffset,
					bounds.Width - xOffset,
					bounds.Height - yOffset
				);
				format.LineAlignment = StringAlignment.Near;
				using (Brush brush = new SolidBrush(ForeColor))
					g.DrawString(item.Details, Font, brush, textRect, format);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			PaintHelper.FillWithParentBackground(e.Graphics, this);
		}
		#endregion
	}
}