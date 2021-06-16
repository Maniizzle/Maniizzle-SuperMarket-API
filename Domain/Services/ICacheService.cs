using System;
using System.Threading.Tasks;

namespace Supermarket.API.Domain.Services
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);
        Task SetCacheValueAsync<T>(string key,T value,TimeSpan? absoluteExpireTime=null);
        
    }
}