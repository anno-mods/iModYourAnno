using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Collections
{
    public interface IQueue<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        T Dequeue();
        void Enqueue(T item);

        int Count();        
    }
}
