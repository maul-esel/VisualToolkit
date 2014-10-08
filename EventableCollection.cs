using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace VisualToolkit
{
	public class EventableCollection<TItem> : IList<TItem> where TItem : INotifyPropertyChanged
	{
		private readonly List<TItem> itemList = new List<TItem>();

		public EventableCollection(bool allowDuplicates)
		{
			this.allowDuplicates = allowDuplicates;
		}

		public EventableCollection()
			: this(true)
		{
		}

		public EventableCollection(IEnumerable<TItem> items, bool allowDuplicates)
			: this(allowDuplicates)
		{
			AddRange(items);
		}

		public EventableCollection(IEnumerable<TItem> items)
			: this(items, true)
		{
		}

		private readonly bool allowDuplicates;

		public bool AllowDuplicates {
			get { return allowDuplicates; }
		}

		public void AddRange(IEnumerable<TItem> items)
		{
			foreach (TItem item in items)
				Add(item);
		}

		#region IList<TItem>
		public int Count {
			get { return itemList.Count; }
		}

		public TItem this[int index] {
			get { return itemList[index]; }
			set {
				RemoveAt(index);
				Insert(index, value);
			}
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public int IndexOf(TItem item)
		{
			return itemList.IndexOf(item);
		}

		public bool Contains(TItem item)
		{
			return itemList.Contains(item);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			itemList.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return itemList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return itemList.GetEnumerator();
		}

		public void Add(TItem item)
		{
			if (AllowDuplicates || !Contains(item)) {
				item.PropertyChanged += OnItemChanged;
				itemList.Add(item);
				OnItemAdded(new ItemEventArgs<TItem>(item));
			}
		}

		public void Insert(int index, TItem item)
		{
			if (AllowDuplicates || !Contains(item)) {
				item.PropertyChanged += OnItemChanged;
				itemList.Insert(index, item);
				OnItemAdded(new ItemEventArgs<TItem>(item));
			} else
				throw new InvalidOperationException("Duplicate item");
		}

		public bool Remove(TItem item)
		{
			if (Contains(item)) {
				item.PropertyChanged -= OnItemChanged;
				return itemList.Remove(item);
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		public void Clear()
		{
			while (Count > 0)
				RemoveAt(0);
		}
		#endregion

		#region events
		public event EventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(EventArgs e)
		{
			EventHandler handler = CollectionChanged;
			if (handler != null)
				handler(this, e);
		}

		public event ItemEventHandler<TItem> ItemAdded;

		protected virtual void OnItemAdded(ItemEventArgs<TItem> e)
		{
			ItemEventHandler<TItem> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
			OnCollectionChanged(e);
		}

		public event ItemEventHandler<TItem> ItemChanged;

		protected virtual void OnItemChanged(ItemEventArgs<TItem> e)
		{
			ItemEventHandler<TItem> handler = ItemChanged;
			if (handler != null)
				handler(this, e);
			OnCollectionChanged(e);
		}

		private void OnItemChanged(object sender, PropertyChangedEventArgs e)
		{
			OnItemChanged(new ItemEventArgs<TItem>((TItem)sender));
		}

		public event ItemEventHandler<TItem> ItemRemoved;

		protected virtual void OnItemRemoved(ItemEventArgs<TItem> e)
		{
			ItemEventHandler<TItem> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
			OnCollectionChanged(e);
		}
		#endregion
	}
}

