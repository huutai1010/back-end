using Common.AppConfiguration;
using Common.Interfaces;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EmptyRedisCacheService : IRedisCacheService
    {
        public Task<TResult?> Get<TResult>(string key)
        {
            return Task.FromResult<TResult?>(default);
        }

        public Task<List<TResult>?> GetList<TResult>(string key)
        {
            // Do nothing
            return Task.FromResult<List<TResult>?>(null);
        }

        public Task RemoveAsync(string key)
        {
            // Do nothing
            return Task.CompletedTask;
        }

        public Task SaveCacheAsync<TResult>(string key, List<TResult> value)
        {
            // Do nothing
            return Task.CompletedTask;
        }

        public Task SaveCacheAsync<TResult>(string key, TResult jsonData)
        {
            return Task.CompletedTask;
        }

        public Task SaveCacheAsync<TResult>(string key, TResult jsonData, double? absoluteExpiration = null, double? slidingExpiration = null)
        {
            return Task.CompletedTask;
        }

        public Task SaveCachePermanentAsync<TResult>(string key, TResult jsonData)
        {
            return Task.CompletedTask;
        }
    }
}
