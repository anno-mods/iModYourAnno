using Imya.Models.NotifyPropertyChanged;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Imya.Models.Collections
{
    //A simple wrapper to have a default queue that implements our IQueue interface
    public class WrappedQueue<T> : IQueue<T>
    {
        private List<T> _list;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public WrappedQueue()
        {
            _list = new List<T>();
        }

        public void Enqueue(T item)
        {
            _list.Add(item);
            CollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public T Dequeue() 
        {
            var item = _list.First();
            _list.RemoveAt(0);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }

        public void Remove(T item)
        {
            var index = _list.IndexOf(item);
            _list.Remove(item);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }


        public int Count() => _list.Count();

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
