using Common.AppConfiguration;
using Common.Interfaces;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly RedisSettings _redisSettings;
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IDistributedCache distributedCache, IOptions<RedisSettings> redisSettings)
        {
            _redisSettings = redisSettings.Value;
            _distributedCache = distributedCache;
        }

        public async Task<List<TResult>?> GetList<TResult>(string key)
        {
            var cacheValue = await _distributedCache.GetAsync(key);
            if (cacheValue == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<List<TResult>>(Encoding.UTF8.GetString(cacheValue));
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task SaveCacheAsync<TResult>(string key, TResult value)
        {
            double absoluteExpiration = _redisSettings.AbsoluteExpirationInMinutes;
            double slidingExpiration = _redisSettings.SlidingExpirationInMinutes;
            var serialized = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var cacheValue = Encoding.UTF8.GetBytes(serialized);
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpiration));
            await _distributedCache.SetAsync(key, cacheValue, options);
        }

        public async Task SaveCacheAsync<TResult>(string key, TResult value, double? absoluteExpiration = null, double? slidingExpiration = null)
        {
            if (absoluteExpiration == null || absoluteExpiration <= 0)
            {
                absoluteExpiration = _redisSettings.AbsoluteExpirationInMinutes;
            }
            if (slidingExpiration == null || slidingExpiration <= 0)
            {
                slidingExpiration = _redisSettings.SlidingExpirationInMinutes;
            }
            var serialized = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var cacheValue = Encoding.UTF8.GetBytes(serialized);
            var options = new DistributedCacheEntryOptions()
                
                .SetAbsoluteExpiration(TimeSpan.FromMinutes((double)absoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes((double)slidingExpiration));
            await _distributedCache.SetAsync(key, cacheValue, options);
        }

        public async Task<TResult?> Get<TResult>(string key)
        {
            var cacheValue = await _distributedCache.GetAsync(key);
            if (cacheValue == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TResult>(Encoding.UTF8.GetString(cacheValue));
        }

        public async Task SaveCachePermanentAsync<TResult>(string key, TResult value)
        {
            var serialized = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var cacheValue = Encoding.UTF8.GetBytes(serialized);
            await _distributedCache.SetAsync(key, cacheValue);
        }
    }
}
