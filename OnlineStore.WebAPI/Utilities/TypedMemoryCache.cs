using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace OnlineStore.WebAPI.Utilities
{
    public class TypedMemoryCache<T, K> where T : struct where K : class
    {
        private readonly IMemoryCache _memoryCache;

        public TypedMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue(T key, out K value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public K Set(T key, K value, MemoryCacheEntryOptions options)
        {
            return _memoryCache.Set(key, value, options);
        }

        public K Get(T key)
        {
            return _memoryCache.Get(key) as K;
        }
    }
}
