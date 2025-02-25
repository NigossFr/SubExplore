using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SubExplore.Services.Interfaces;

namespace SubExplore.Services.Implementations
{
    /// <summary>
    /// Implémentation du service de cache utilisant IMemoryCache
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _defaultOptions;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
            _defaultOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                // IMemoryCache est synchrone, mais on garde l'interface asynchrone pour cohérence
                var value = _cache.Get<T>(key);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cache get error: {ex.Message}");
                return Task.FromResult(default(T));
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = expiration.HasValue
                    ? new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration.Value,
                        SlidingExpiration = TimeSpan.FromMinutes(Math.Min(expiration.Value.TotalMinutes / 2, 10))
                    }
                    : _defaultOptions;

                _cache.Set(key, value, options);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cache set error: {ex.Message}");
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cache remove error: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public Task ClearAsync()
        {
            try
            {
                if (_cache is MemoryCache memoryCache)
                {
                    memoryCache.Compact(1.0);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cache clear error: {ex.Message}");
            }
            return Task.CompletedTask;
        }
    }
}
