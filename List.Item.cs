using System;
using System.ComponentModel;
using System.Drawing;

namespace VisualToolkit
{
	partial class List
	{
		public class Item : INotifyPropertyChanged
		{
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

			private string details;

			public string Details {
				get { return details; }
				set {
					if (details != value) {
						details = value;
						OnPropertyChanged(new PropertyChangedEventArgs("Details"));
					}
				}
			}

			private Image image;

			public Image Image {
				get { return image; }
				set {
					if (image != value) {
						image = value;
						OnPropertyChanged(new PropertyChangedEventArgs("Image"));
					}
				}
			}

			private Group group;

			public Group Group {
				get { return group; }
				set {
					if (group != value) {
						if (value != null)
							value.Items.Add(this);
						if (group != null)
							group.Items.Remove(this);
						group = value;
						OnPropertyChanged(new PropertyChangedEventArgs("Group"));
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