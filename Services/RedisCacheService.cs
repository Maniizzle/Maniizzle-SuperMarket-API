using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Supermarket.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Supermarket.API.Services
{
    public class RedisCacheService : ICacheService
    {
        //private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly IDistributedCache cache;

        public RedisCacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }
        public async Task<string> GetCacheValueAsync(string key)
        {
            //using connection multiplexer style
            // var db = connectionMultiplexer.GetDatabase();
            //return await db.StringGetAsync(key);

            return await cache.GetStringAsync(key);
        }

        public async Task SetCacheValueAsync<T>(string key, T value,TimeSpan? absolute=null)
        {
            //using connection multiplexer style
            //var db = connectionMultiplexer.GetDatabase();
            //return await db.StringSetAsync(key,value);
           
            var abstime = absolute ?? TimeSpan.FromSeconds(60);
            var data = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = abstime
            };
            await cache.SetStringAsync(key, data, options);
        }
    }
}
