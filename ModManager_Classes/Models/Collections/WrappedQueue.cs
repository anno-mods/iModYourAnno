using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Imya.Models.Collections
{
    //A simple wrapper to have a default queue that implements our IQueue interface
    public class WrappedQueue<T> : IQueue<T>
    {
        private Queue<T> _queue;

        public WrappedQueue()
        { 
            _queue = new Queue<T>();
        }

        public void Enqueue(T item)
        { 
            _queue.Enqueue(item);
        }

        public T Dequeue() => _queue.Dequeue();

        public int Count() => _queue.Count();

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
