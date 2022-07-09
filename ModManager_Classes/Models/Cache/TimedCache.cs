using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Cache
{
    internal class TimedCache<TKey, TResult> : ITimedCache<TKey, TResult> where TKey : notnull
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromSeconds(60);

        private MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        public TimedCache()
        {

        }

        public async Task<TResult> GetOrCreateAsync(TKey key, Func<ICacheEntry, Task<TResult>> factory)
        {
            return await memoryCache.GetOrCreateAsync(key, factory);
        }

        public TResult GetOrCreate(TKey key, Func<ICacheEntry, TResult> factory)
        {
            return memoryCache.GetOrCreate(key, factory);
        }
    }
}
