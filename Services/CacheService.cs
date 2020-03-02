using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;


namespace MyFirstStore.Services
{
    public class CacheService
    {
        private IMemoryCache _memoryCache;
        private static List<string> keys;
        public CacheService(IMemoryCache memoryCache)
        {
            keys = new List<string>();
            _memoryCache = memoryCache;
        }
        public void Set<T>(string keyName, T value)
        {
            keyName = keyName.ToLower();
            _memoryCache.Set(keyName, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            keys.Add(keyName);
        }
        public T Get<T>(string keyName)
        {
            keyName = keyName.ToLower();
            _memoryCache.TryGetValue(keyName, out var value);
            if (value!=null && value.GetType() == typeof(T))
            {
                return (T)value;
            }
            else
            {
                return default;
            }
        }
        public bool Remove(string keyName)
        {
            keyName = keyName.ToLower();
            List<string> removeKeys = new List<string>();
            foreach (var iKey in keys)
            {
                if (iKey.Contains(keyName))
                {
                    if (_memoryCache.TryGetValue(iKey, out _))
                    {
                        _memoryCache.Remove(iKey);
                    }
                    removeKeys.Add(iKey);
                }
            }
            foreach (var rKey in removeKeys)
            {
                keys.Remove(rKey);
            }
            return true;
        }
 
    }
}
