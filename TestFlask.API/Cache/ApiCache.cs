using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace TestFlask.API.Cache
{
    public static class ApiCache
    {
        private static MemoryCache memoryCache = MemoryCache.Default;

        public static T Get<T>(string key)
        {
            return (T)memoryCache.Get(key);
        }

        public static bool Add(string key, object value)
        {
            return Add(key, value, DateTimeOffset.UtcNow.AddMinutes(5));
        }

        public static bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            return memoryCache.Add(key, value, absExpiration);
        }

        public static void Delete(string key)
        {
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }
    }
}