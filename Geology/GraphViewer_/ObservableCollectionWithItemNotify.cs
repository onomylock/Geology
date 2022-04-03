using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Geology.GraphViewer
{
    public class ObservableCollectionWithItemNotify<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        // https!://stackoverflow.com/questions/901921/observablecollection-and-item-propertychanged

        public ObservableCollectionWithItemNotify()
        {
            this.CollectionChanged += items_CollectionChanged;
        }

        public ObservableCollectionWithItemNotify(IEnumerable<T> collection) : base(collection)
        {
            this.CollectionChanged += items_CollectionChanged;
            foreach (INotifyPropertyChanged item in collection)
                item.PropertyChanged += item_PropertyChanged;
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.OldItems != null)
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= item_PropertyChanged;

                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += item_PropertyChanged;
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            this.OnCollectionChanged(reset);
        }

        protected override void ClearItems()
        {
            // => расскомментировать[x], но тогда не будет обновляться событие item_PropertyChanged().
            //List<T> removed = new List<T>(this);
            //base.ClearItems(); // -> reset
            //base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));

            while (Count > 0)
                base.RemoveAt(Count - 1);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // [x]
            //if (e.Action != NotifyCollectionChangedAction.Reset)
            base.OnCollectionChanged(e);
        }
    }

}
