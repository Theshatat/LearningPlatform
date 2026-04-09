using LearningPlatform.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace LearningPlatform.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                cachedValue = await factory();

                _cache.Set(cacheKey, cachedValue, TimeSpan.FromMinutes(5));
            }
            return cachedValue;
        }
    }
}
