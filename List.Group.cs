using System;

namespace VisualToolkit
{
	partial class List
	{
		public class Group : NotifyPropertyChangedBase
		{
			private readonly EventableCollection<Item> items = new EventableCollection<Item>(false);

			public EventableCollection<Item> Items {
				get { return items; }
			}

			public Group()
			{
				items.ItemAdded += (s, e) => {
					e.Item.Group = this;
					e.Item.Invoked += OnItemInvoked;
				};
				items.ItemRemoved += (s, e) => {
					e.Item.Group = null;
					e.Item.Invoked -= OnItemInvoked;
				};
				items.CollectionChanged += (s, e) => OnPropertyChanged("Items");
			}

			#region ItemInvoked
			public event ItemEventHandler<Item> ItemInvoked;

			private void OnItemInvoked(object sender, EventArgs e)
			{
				OnItemInvoked(new ItemEventArgs<Item>(sender as Item));
			}

			protected virtual void OnItemInvoked(ItemEventArgs<Item> e)
			{
				ItemEventHandler<Item> handler = ItemInvoked;
				if (handler != null)
					handler(this, e);
			}
			#endregion

			private string title;

			public string Title {
				get { return title; }
				set {
					if (title != value) {
						title = value;
						OnPropertyChanged("Title");
					}
				}
			}

			private object tag;

			public object Tag {
				get { return tag; }
				set {
					if (tag != value) {
						tag = value;
						OnPropertyChanged("Tag");
					}
				}
			}
		}
	}
}