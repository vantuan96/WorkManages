using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Kztek_Library.Helpers
{
    public class CacheHelper
    {
        IMemoryCache Cache;

        public CacheHelper(IMemoryCache Cache)
        {
            this.Cache = Cache;
        }

        // /// <summary>
        // /// Retrieve cached item
        // /// </summary>
        // /// <typeparam name="T">Type of cached item</typeparam>
        // /// <param name="key">Name of cached item</param>
        // /// <returns>Cached item as type</returns>
        // public static T Get<T>(string key) where T : class
        // {
        //     //var Cache = new MemoryCache(new MemoryCacheOptions());

        //     return Cache.Get<T>(key);
        // }

        // /// <summary>
        // /// Retrieve cached item
        // /// </summary>
        // /// <typeparam name="T">Type of cached item</typeparam>
        // /// <param name="key">Name of cached item</param>
        // /// <returns>Cached item as type</returns>
        // public static bool TryGet<T>(string key, out T data) where T : class
        // {
        //     //var Cache = new MemoryCache(new MemoryCacheOptions());

        //     return Cache.TryGetValue<T>(key, out data);
        // }

        // /// <summary>
        // /// Insert value into the cache using
        // /// appropriate name/value pairs
        // /// </summary>
        // /// <typeparam name="T">Type of cached item</typeparam>
        // /// <param name="objectToCache">Item to be cached</param>
        // /// <param name="key">Name of item</param>
        // public static void Add<T>(string key, T data, int cacheTime) where T : class
        // {
        //     try
        //     {
        //         //var Cache = new MemoryCache(new MemoryCacheOptions());

        //         if (data == null)
        //             return;

        //         MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
        //         cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(30);
        //         cacheExpirationOptions.Priority = CacheItemPriority.Normal;

        //         Cache.Set<T>(key, data, cacheExpirationOptions);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        // /// <summary>
        // /// Remove item from cache
        // /// </summary>
        // /// <param name="key">Name of cached item</param>
        // public static void Clear(string key)
        // {
        //     //var Cache = new MemoryCache(new MemoryCacheOptions());
        //     Cache.Remove(key);
        // }
    }
}