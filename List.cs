using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace VisualToolkit
{
	public partial class List : ScrollableControl
	{
		public List()
			: base()
		{
			itemList.CollectionChanged += OnItemsChanged;
			groupList.CollectionChanged += OnGroupsChanged;

			ItemHeight = 50;
			ItemBorderWidth = 2;

			ForeColor       = ColorTheme.DefaultTheme.TextColor;
			ItemBackColor   = ColorTheme.DefaultTheme.BackColor;
			ItemBorderColor = ColorTheme.DefaultTheme.BorderColor;

			SelectedItemForeColor   = ColorTheme.DefaultTheme.TextColor;
			SelectedItemBackColor   = ColorTheme.DefaultTheme.BackColor;
			SelectedItemBorderColor = ColorTheme.DefaultTheme.BorderColor;
		}

		public List(IEnumerable<Item> items)
			: this()
		{
			itemList.AddRange(items);
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

		public InvocationMode InvocationMode {
			get;
			set;
		}

		#region painting
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			PaintHelper.FillWithParentBackground(e.Graphics, this);
		}
		#endregion
	}
}