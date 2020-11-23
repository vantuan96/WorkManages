using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Kztek_Library.Functions
{
    public class CacheFunction
    {

        public static bool TryGet<T>(HttpContext context, string key, out T data)
        {
            var Cache = context.RequestServices.GetService<IMemoryCache>();
            var result = Cache.TryGetValue<T>(key, out data);
            return result;
        }

        public static void Add<T>(HttpContext context, string key, T data, int cacheTime) where T : class
        {
            var Cache = context.RequestServices.GetService<IMemoryCache>();
            try
            {
                if (data == null)
                    return;

                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
                cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(cacheTime);
                cacheExpirationOptions.Priority = CacheItemPriority.Normal;

                Cache.Set<T>(key, data, cacheExpirationOptions);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static void Clear(HttpContext context, string key)
        {
            var Cache = context.RequestServices.GetService<IMemoryCache>();
            Cache.Remove(key);
        }
    }
}