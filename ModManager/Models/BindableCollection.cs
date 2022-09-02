using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Imya.UI.Models
{
    public class BindableCollection<TItemModel> : BindableCollection<TItemModel, TItemModel, IReadOnlyCollection<TItemModel>> where TItemModel : class
    {
        public BindableCollection(IReadOnlyCollection<TItemModel> collection, DispatcherObject context) : base(collection, context, (x, c) => x)
        {
            // Sometimes we don't need the items to be bindable.
        }
    }

    public class BindableCollection<TItemModel, TItemBindable, TCollectionModel> : Bindable<TCollectionModel>, IReadOnlyCollection<TItemBindable>, INotifyCollectionChanged
        where TCollectionModel : class, IReadOnlyCollection<TItemModel> where TItemModel : class
    {
        #region IReadOnlyCollection, INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public int Count => Model.Count;
        public IEnumerator<TItemBindable> GetEnumerator() => _collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
        #endregion

        #region Order and Filter
        public IComparer<TItemModel>? Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    Collection_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        private IComparer<TItemModel>? _order;

        public Func<TItemModel, bool>? Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Collection_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        private Func<TItemModel, bool>? _filter;
        #endregion

        private IReadOnlyCollection<TItemBindable> _collection;
        private readonly Func<TItemModel, DispatcherObject, TItemBindable> _itemCreator;

        public BindableCollection(TCollectionModel collection, DispatcherObject context, Func<TItemModel, DispatcherObject, TItemBindable> itemCreator) : base(collection, context)
        {
            _itemCreator = itemCreator;
            _collection = new Collection<TItemBindable>(Model.Select(x => _itemCreator(x, _context)).ToList());

            // use weak events as we do not own the model and don't want the hassle to figure out when to remove it
            if (Model is INotifyCollectionChanged notifying)
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>.AddHandler(notifying, nameof(CollectionChanged), Collection_CollectionChanged);
        }

        private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<TItemModel> filtered = _filter is not null ? Model.Where(x => _filter(x)) : Model;
            IEnumerable<TItemModel> ordered = _order is not null ? filtered.OrderBy(x => x, _order) : filtered;

            if (e.Action == NotifyCollectionChangedAction.Replace && typeof(Bindable<TItemModel>).IsAssignableFrom(typeof(TItemBindable)))
            {
                // this is a little complex but by reusing TItemBindable objects we avoid hassle with keeping selection alive
                Dictionary<TItemModel, TItemBindable> reuseItems = new(_collection.Select(x => new KeyValuePair<TItemModel, TItemBindable>((x as Bindable<TItemModel>)!.Model, x)));
                _collection = new Collection<TItemBindable>(ordered.Select(x => reuseItems.ContainsKey(x) ? reuseItems[x] : _itemCreator(x, _context)).ToList());
            }
            else
            {
                _collection = new Collection<TItemBindable>(ordered.Select(x => _itemCreator(x, _context)).ToList());
            }

            if (_context.Dispatcher.CheckAccess())
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                _context.Dispatcher.Invoke(() =>
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                });
            }
        }
    }
}
