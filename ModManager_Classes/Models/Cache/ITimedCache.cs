using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Cache
{
    public interface ITimedCache<TKey, TResult> : ICache<TKey, TResult>
    { 
        public TimeSpan ExpirationTime { get; set; }
    }
}
