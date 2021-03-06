﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace BISTickerAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public object GetCachedObject(object key)
        {
            //Console.WriteLine($"Getting key: {key} from cache");
            return _cache.Get(key);
        }

        public void RemoveCachedObject(object key)
        {
            //Console.WriteLine($"Removing key: {key} from cache");
            _cache.Remove(key);
        }

        public void AddCachedObject(object key, object value)
        {
            //Console.WriteLine($"Putting stuff into cache, key: {key}, value: {value}");
            _cache.Set(key, value);
        }

        public static string GenerateCacheKeyForPair(string baseKey, string coin1, string coin2)
        {
            return $"{baseKey}-{coin1}-{coin2}";
        }
    }

    public static class CacheKeys
    {
        public static string TickerResult { get { return "ticker-result"; } }
        public static string MarketsResult { get { return "markets-result"; } }
        public static string Visits { get { return "visits"; } }
        public static string PairsMap => "currency_pairs";
    }
}
