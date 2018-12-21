using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Entities;
using BISTickerAPI.Model;
using BISTickerAPI.Model.POCO;

namespace BISTickerAPI.Services.TradeSatoshi
{
    public class TradeSatoshiTickerService : ITicker
    {
        private readonly TradeSatoshiAPI _api;
        private readonly TickerDbContext _dbContext;

        public TradeSatoshiTickerService(TickerDbContext dbContext, TradeSatoshiAPI api)
        {
            _api = api;
            _dbContext = dbContext;
        }

        public bool UpdateTicker(string[] currencyPairs)
        {
            var data = _api.FetchMarkets();
            var exchange = _dbContext.Exchanges.FirstOrDefault(e => e.Name.Equals(GetExchangeName()));
            var coins = _dbContext.Coins.ToList();

            foreach (var pair in currencyPairs)
            {
                var split = pair.Split('/');
                var ticker = CreateTickerEntry(data, exchange,
                                coins.SingleOrDefault(c => c.Symbol.Equals(split[0])),
                                coins.SingleOrDefault(c => c.Symbol.Equals(split[1]))
                             );

                if (ticker == null)
                {
#if DEBUG
                    Console.WriteLine($"TradeSatoshi ticker does not have pair {pair}.");
#endif
                    continue;
                    //throw new Exception($"Ticker for pair {pair} got some issue!");
                }

                _dbContext.Add(ticker);
            }

            _dbContext.SaveChanges();

            return true;
        }

        protected TickerEntry CreateTickerEntry(List<TradeSatoshiMarkets> tickers, Exchange exchange, Coin mainCoin, Coin baseCoin)
        {
            var tickerSymbol = $"{mainCoin.Symbol}_{baseCoin.Symbol}";
            var ticker = tickers.Find(t => t.Label.Equals(tickerSymbol));
            if (ticker == null)
            {
                return null;
            }

            var tickerEntry = new TickerEntry()
            {
                AskPrice = ticker.Ask,
                BidPrice = ticker.Bid,
                Change = ticker.Change,
                High = ticker.High,
                Low = ticker.Low,
                Volume = ticker.Volume,
                BaseVolume = ticker.VolumeBase,
                Open = null,
                Label = mainCoin.Symbol,
                LastPrice = ticker.Last,
                Timestamp = DateTime.UtcNow,
                Exchange = exchange,
                PairCoin1 = mainCoin,
                PairCoin2 = baseCoin
            };

            return tickerEntry;
        }

        public string GetExchangeName()
        {
            return "TradeSatoshi";
        }
    }
}
