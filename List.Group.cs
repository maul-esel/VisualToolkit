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
				items.ItemAdded += (s, e) => e.Item.Group = this;
				items.ItemRemoved += (s, e) => e.Item.Group = null;
				items.CollectionChanged += (s, e) => OnPropertyChanged("Items");
			}

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