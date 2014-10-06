using System;

namespace VisualToolkit
{
	public delegate void ItemEventHandler<TItem>(object sender, ItemEventArgs<TItem> e);

	public class ItemEventArgs<TItem> : EventArgs
	{
		private readonly TItem item;

		public TItem Item {
			get { return item; }
		}

		public ItemEventArgs(TItem item)
		{
			this.item = item;
		}
	}
}