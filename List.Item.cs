using System.Drawing;

namespace VisualToolkit
{
	partial class List
	{
		public class Item : NotifyPropertyChangedBase
		{
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

			private string details;

			public string Details {
				get { return details; }
				set {
					if (details != value) {
						details = value;
						OnPropertyChanged("Details");
					}
				}
			}

			private Image image;

			public Image Image {
				get { return image; }
				set {
					if (image != value) {
						image = value;
						OnPropertyChanged("Image");
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
						OnPropertyChanged("Group");
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