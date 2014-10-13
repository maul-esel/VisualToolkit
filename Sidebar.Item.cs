using System;

namespace VisualToolkit
{
	partial class Sidebar
	{
		public class Item : NotifyPropertyChangedBase
		{
			private string text;

			public string Text {
				get { return text; }
				set {
					if (text != value) {
						text = value;
						OnPropertyChanged("Text");
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

			public event EventHandler Invoked;

			internal void RaiseInvoked()
			{
				OnInvoked(EventArgs.Empty);
			}

			protected virtual void OnInvoked(EventArgs e)
			{
				EventHandler handler = Invoked;
				if (handler != null)
					handler(this, e);
			}
		}
	}
}