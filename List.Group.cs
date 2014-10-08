using System.ComponentModel;

namespace VisualToolkit
{
	partial class List
	{
		public class Group : INotifyPropertyChanged
		{
			private readonly EventableCollection<Item> items = new EventableCollection<Item>(false);

			public EventableCollection<Item> Items {
				get { return items; }
			}

			public Group()
			{
				items.ItemAdded += (s, e) => e.Item.Group = this;
				items.ItemRemoved += (s, e) => e.Item.Group = null;
				items.CollectionChanged += (s, e) => OnPropertyChanged(new PropertyChangedEventArgs("Items"));
			}

			private string title;

			public string Title {
				get { return title; }
				set {
					if (title != value) {
						title = value;
						OnPropertyChanged(new PropertyChangedEventArgs("Title"));
					}
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
					handler(this, e);
			}
		}
	}
}