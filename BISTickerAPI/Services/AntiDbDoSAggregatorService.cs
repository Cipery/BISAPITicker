using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BISTickerAPI.Services
{
    /// <summary>
    /// This is a higher order class that should sit above AggregatorService, it checks requests against in-memory
    /// app settings - if requested coin pair is not being tracked by this app instace, it returns an error object and never
    /// does any DB query.
    /// Reason for creating this class is prevention of (very simple) DoS attacks
    /// ! Do not forget that saving the ErrorResponse object into memory cache could be used as DoS attack - filling up the memory
    /// with random coins and adding zilions of entries into memory cache. So if this object is surrounded by some kind of cache, save the
    /// Error response! 
    /// </summary>
    public class AntiDbDoSAggregatorService : IAggregatorService
    {
        protected AggregatorService AggregatorService;
        protected AppSettings AppSettings;
        //TODO: Extract this elsewhere
        public static readonly object ErrorResponse =
            JsonConvert.SerializeObject(new {success = false, message = "There is no data for this pair"});

        public AntiDbDoSAggregatorService(AggregatorService aggregatorService, IOptions<AppSettings> appSettings)
        {
            AggregatorService = aggregatorService;
            AppSettings = appSettings.Value;
        }

        public object GetAveragedOuput(string mainCoin, string baseCoin)
        {
            var ticker = $"{mainCoin.ToUpper()}/{baseCoin.ToUpper()}";
            return !AppSettings.FetchPairs.Contains(ticker) ? ErrorResponse : AggregatorService.GetAveragedOuput(mainCoin, baseCoin);
        }

        public object GetPerExchangeOutput(string mainCoin, string baseCoin)
        {
            var ticker = $"{mainCoin.ToUpper()}/{baseCoin.ToUpper()}";
            return !AppSettings.FetchPairs.Contains(ticker) ? ErrorResponse : AggregatorService.GetPerExchangeOutput(mainCoin, baseCoin);
        }

        public void UpdateTickers()
        {
            AggregatorService.UpdateTickers();;
        }
    }
}
