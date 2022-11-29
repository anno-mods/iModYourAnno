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
        private Queue<T> _queue;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public WrappedQueue()
        {
            _queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            CollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public T Dequeue() 
        {
            var item = _queue.Dequeue();
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }


        public int Count() => _queue.Count();

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
