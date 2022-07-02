using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Cache
{
    public interface ICache<TKey, TResult>
    {
        Task<TResult> GetOrCreateAsync(TKey key, Func<ICacheEntry, Task<TResult>> factory);

        TResult GetOrCreate(TKey key, Func<ICacheEntry, TResult> factory);
    }
}
