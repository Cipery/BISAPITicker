using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Entities;
using BISTickerAPI.Helpers;
using BISTickerAPI.Model;
using BISTickerAPI.Model.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BISTickerAPI.Services
{
    public class AggregatorService
    {
        protected readonly TickerDbContext dbContext;
        protected readonly ICacheService memoryCache;
        protected readonly List<ITicker> tickers = new List<ITicker>();
        protected readonly AppSettings settings;
        protected readonly ILogger<AggregatorService> logger;

        public AggregatorService(TickerDbContext dbContext, ICacheService memoryCache, IOptions<AppSettings> settings,
            ILogger<AggregatorService> logger,
            CryptopiaTickerService cryptopiaTicker,
            QTradeTickerService qTradeTicker)
        {
            this.dbContext = dbContext;
            this.memoryCache = memoryCache;
            this.settings = settings.Value;
            this.logger = logger;

            tickers.Add(cryptopiaTicker);
            tickers.Add(qTradeTicker);
        }

        public void UpdateTickers()
        {
            // ok here is the thing, we kinda want to fetch the BTC/USDT price from.. for now only Cryptopia
            // thats why we remove the BTC/USDT pair from "all pairs".
            // so TODO: :))
            var pairs = settings.FetchPairs.Where(p => !p.Equals("BTC/USDT")).ToArray();
            foreach (var ticker in tickers)
            {
                try
                {
                    ticker.UpdateTicker(ticker.GetExchangeName() == "Cryptopia" ? settings.FetchPairs : pairs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    logger.LogError(e, $"Problem while updating ticker {ticker.GetExchangeName()}");
                }
            }

            //invalidateKeys.ForEach(key => cacheService.RemoveCachedObject(key));
        }

        /// <summary>
        /// Gets the data of mainCoin_baseCoin pair and transforms them into json object
        /// that is ready to be returned by request response
        /// </summary>
        /// <param name="mainCoin"></param>
        /// <param name="baseCoin"></param>
        /// <returns></returns>
        public object GetAveragedOuput(string mainCoin, string baseCoin)
        {
            var coins = GetCoins(mainCoin, baseCoin);
#region TODO
            // Why no groupby? Because .net core 2.2 doesnt translate groupby to SQL, so it does it locally.. 
            // I am afraid that it's pulling much more data from db than needed. So getting last record for each exchange is probably faster?
            // test this!
            /*var zkurvel = dbContext.TickerEntries
                .GroupBy(group => group.Exchange.Name)
                .Select(group => group.Where(t => t.PairCoin1 == coins.Item1 && t.PairCoin2 == coins.Item2)
                    .OrderByDescending(p => p.Id).FirstOrDefault());
            var zkurwList = zkurvel.ToList();
            /*.Where(t => t.PairCoin1.Symbol == mainCoin && t.PairCoin2.Symbol == baseCoin)
            .OrderByDescending(t => t.Id);
            .FirstOrDefault();*/
#endregion
            var basePrice = GetTickerEntry(coins.Item2, GetCoin("USDT"));
            var entries = dbContext.Exchanges.ToList().Select(exchange => GetTickerEntry(coins.Item1, coins.Item2, exchange)).ToList();

            //remove possible null entries
            //TODO: make this in less retarded way maybe??
            while (entries.Contains(null))
            {
                entries.Remove(null);
            }

            if (!entries.Any())
            {
                return JsonConvert.SerializeObject(new { success = false, message = "There is no data for this pair" }, Formatting.Indented);
            }

            var coinEntry = new TickerEntry()
            {
                AskPrice = entries.Where(p => !double.IsNaN(p.AskPrice)).Average(p => p.AskPrice),
                BidPrice = entries.Where(p => !double.IsNaN(p.BidPrice)).Average(p => p.BidPrice),
                LastPrice = entries.Where(p => !double.IsNaN(p.LastPrice)).Average(p => p.LastPrice),
                BaseVolume = entries.Where(p => p.BaseVolume.HasValue).Sum(p => p.BaseVolume),
                BuyBaseVolume = entries.Where(p => p.BuyBaseVolume.HasValue).Sum(p => p.BuyBaseVolume),
                SellBaseVolume = entries.Where(p => p.SellBaseVolume.HasValue).Sum(p => p.SellBaseVolume),
                Volume = entries.Where(p => p.Volume.HasValue).Sum(p => p.Volume),
                BuyVolume = entries.Where(p => p.BuyVolume.HasValue).Sum(p => p.BuyVolume),
                SellVolume = entries.Where(p => p.SellVolume.HasValue).Sum(p => p.SellVolume),
                Change = entries.Where(p => p.Change.HasValue).Average(p => p.Change),
                Low = entries.Where(p => p.Low.HasValue).OrderBy(p =>p.Low).FirstOrDefault()?.Low,
                High = entries.Where(p => p.High.HasValue).OrderByDescending(p => p.Low).FirstOrDefault()?.High,
                Open = entries.Where(p => p.Open.HasValue).Average(p => p.Open),
                Close = entries.Where(p => p.Close.HasValue).Average(p => p.Close),
                PairCoin1 = coins.Item1,
                PairCoin2 = coins.Item2,
                Label = entries.FirstOrDefault()?.Label,
                Timestamp = entries.OrderBy(p => p.Timestamp).First().Timestamp
            };

            var responseObject = GenerateHeaderObject(coinEntry, basePrice);
            responseObject.quotes = GenerateQuotes(coinEntry, basePrice);


            return JsonConvert.SerializeObject(responseObject, Formatting.Indented);
            //return responseObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainCoin"></param>
        /// <param name="baseCoin"></param>
        /// <returns></returns>
        public object GetPerExchangeOutput(string mainCoin, string baseCoin)
        {
            var coins = GetCoins(mainCoin, baseCoin);
            var baseCoinTickerEntry = GetTickerEntry(coins.Item2, GetCoin("USDT"));
            var exchanges = dbContext.Exchanges.ToList();
            var dict = new Dictionary<string, object>();
            exchanges.ForEach(exchange =>
            {
                var tickerEntry = GetTickerEntry(coins.Item1, coins.Item2, exchange);
                if (tickerEntry == null)
                {
                    return;
                }
                var result = new Dictionary<string, object>();
                result["timestamp"] = new DateTimeOffset(tickerEntry.Timestamp).ToUnixTimeSeconds();
                result["volume"] = tickerEntry.Volume;
                foreach (var keyValuePair in GenerateQuotes(tickerEntry, baseCoinTickerEntry, false, true))
                {
                    result[keyValuePair.Key] = keyValuePair.Value;
                }
                dict.Add(exchange.Name,result);
            });
                
            if (!dict.Any())
            {
                JsonConvert.SerializeObject(new { success = false, message = "There is no data for this pair" }, Formatting.Indented);
            }

            return JsonConvert.SerializeObject(new
            {
                success = true,
                name = coins.Item1.Name,
                symbol = coins.Item1.Symbol,
                markets = dict
            }, Formatting.Indented);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainEntry"></param>
        /// <param name="baseCoinEntry"></param>
        /// <param name="includeTimestamp"></param>
        /// <param name="includeVolumes"></param>
        /// <returns></returns>
        private IDictionary<string, object> GenerateQuotes(TickerEntry mainEntry, TickerEntry baseCoinEntry, bool includeTimestamp = false, bool includeVolumes = false)
        {
            // Why all the decimals? Cause current MYSQL provider (POMELO) doesnt allow for decimal(24,8) types
            var quotes = new Dictionary<string, object>();
            dynamic btcQuote = new ExpandoObject();
            btcQuote.askPrice = (decimal) mainEntry.AskPrice;
            btcQuote.bidPrice = (decimal) mainEntry.BidPrice;
            btcQuote.lastPrice = (decimal) mainEntry.LastPrice;
            if (includeVolumes && mainEntry.BaseVolume != null) btcQuote.volume = (decimal) mainEntry.BaseVolume.Value;
            if (includeTimestamp)
            {
                btcQuote.timestamp = new DateTimeOffset(mainEntry.Timestamp).ToUnixTimeSeconds();
            }

            dynamic usdQuote = new ExpandoObject();
            usdQuote.askPrice = Math.Round((decimal) mainEntry.AskPrice * (decimal) baseCoinEntry.LastPrice, 3,
                MidpointRounding.ToEven);
            usdQuote.bidPrice = Math.Round((decimal) mainEntry.BidPrice * (decimal) baseCoinEntry.LastPrice, 3,
                MidpointRounding.ToEven);
            usdQuote.lastPrice = Math.Round((decimal) mainEntry.LastPrice * (decimal) baseCoinEntry.LastPrice, 3,
                MidpointRounding.ToEven);


            if (includeVolumes && mainEntry.Volume != null) usdQuote.volume = usdQuote.lastPrice * Math.Round((decimal) mainEntry.Volume.Value, 3, MidpointRounding.ToEven);

            quotes.Add("BTC", btcQuote);
            quotes.Add("USD", usdQuote);
            return quotes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainEntry"></param>
        /// <param name="baseCoinEntry"></param>
        /// <returns></returns>
        private dynamic GenerateHeaderObject(TickerEntry mainEntry, TickerEntry baseCoinEntry)
        {
            dynamic headerObject = new ExpandoObject();
            headerObject.success = true;
            headerObject.name = mainEntry.PairCoin1.Name;
            headerObject.symbol = mainEntry.PairCoin1.Symbol;
            headerObject.timestamp = new DateTimeOffset(mainEntry.Timestamp).ToUnixTimeSeconds();
            headerObject.volume24h = mainEntry.Volume;
            headerObject.volumeBTC24h = mainEntry.BaseVolume;
            headerObject.volumeUSD24h = Math.Round(mainEntry.BaseVolume.Value * baseCoinEntry.LastPrice, 3,
                MidpointRounding.ToEven);
            headerObject.priceChange24h = mainEntry.Change;
            return headerObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="baseCoin"></param>
        /// <returns></returns>
        public TickerEntry GetTickerEntry(Coin coin, Coin baseCoin)
        {
            return dbContext.TickerEntries.Where(t => t.PairCoin1 == coin && t.PairCoin2 == baseCoin)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
        }

        public TickerEntry GetTickerEntry(Coin coin, Coin baseCoin, Exchange exchange)
        {
            return dbContext.TickerEntries.Where(t => t.PairCoin1 == coin && t.PairCoin2 == baseCoin && t.Exchange == exchange)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
        }

        public Coin GetCoin(string coinSymbol)
        {
            var coin = dbContext.Coins.SingleOrDefault(c => c.Symbol == coinSymbol);
            if (coin == null)
            {
                throw new CoinNotFoundException {CoinSymbol = coinSymbol};
            }

            return coin;
        }

        public (Coin, Coin) GetCoins(string leftCoin, string rightCoin)
        {
            var coin = GetCoin(leftCoin);
            var baseCoin = GetCoin(rightCoin);

            return (coin, baseCoin);
        }

        public string GenerateCacheKeyForPair(string coin1, string coin2)
        {
            return $"{CacheKeys.TickerResult}-{coin1}-{coin2}";
        }
    }
}
