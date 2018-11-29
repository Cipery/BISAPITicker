using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Helpers;
using BISTickerAPI.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BISTickerAPI.Services
{
    /// <summary>
    /// This class is just a wrapper around AggregatorService, it caches its results into memory and then returns them, if there are
    /// any
    /// </summary>
    public class MemoryCachingAggregatorService : IAggregatorService
    {
        protected AntiDbDoSAggregatorService AggregatorService;
        protected CacheService CacheService;
        protected AppSettings AppSettings;

        public MemoryCachingAggregatorService(CacheService cacheService, AntiDbDoSAggregatorService aggregatorService, IOptions<AppSettings> settings)
        {
            AggregatorService = aggregatorService;
            CacheService = cacheService;
            AppSettings = settings.Value;
        }

        public object GetAveragedOuput(string mainCoin, string baseCoin)
        {
            var key = CacheService.GenerateCacheKeyForPair(CacheKeys.TickerResult, mainCoin, baseCoin);
            var cacheObject = CacheService.GetCachedObject(key);

            if (cacheObject == null)
            {
                cacheObject = AggregatorService.GetAveragedOuput(mainCoin, baseCoin);
                // We do NOT store anything in memory cache for unknown coins!
                if (cacheObject != AntiDbDoSAggregatorService.ErrorResponse)
                {
                    CacheService.AddCachedObject(key, cacheObject);
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Cache hit!");
#endif
            }

            return cacheObject;
        }

        public object GetPerExchangeOutput(string mainCoin, string baseCoin)
        {
            var key = CacheService.GenerateCacheKeyForPair(CacheKeys.MarketsResult, mainCoin, baseCoin);
            var cacheObject = CacheService.GetCachedObject(key);

            if (cacheObject == null)
            {
                cacheObject = AggregatorService.GetPerExchangeOutput(mainCoin, baseCoin);
                // We do NOT store anything in memory cache for unknown coins!
                if (cacheObject != AntiDbDoSAggregatorService.ErrorResponse)
                {
                    CacheService.AddCachedObject(key, cacheObject);
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Cache hit!");
#endif
            }

            return cacheObject;
        }

        public void UpdateTickers()
        {
            AggregatorService.UpdateTickers();
            //TODO: Think about this cache invalidation
            // cache manager could hold a list of base keys (eg: "key-something-%") and invalidate all of these and clear that list after that
            // but that would increase number of operations needed to get item from cache. And to cache.
            foreach (var appSettingsFetchPair in AppSettings.FetchPairs)
            {
                var pairSplit = appSettingsFetchPair.SplitSlashedPair();
                CacheService.RemoveCachedObject(CacheService.GenerateCacheKeyForPair(CacheKeys.TickerResult, pairSplit[0], pairSplit[1]));
                CacheService.RemoveCachedObject(CacheService.GenerateCacheKeyForPair(CacheKeys.MarketsResult, pairSplit[0], pairSplit[1]));
            }
        }
    }
}
