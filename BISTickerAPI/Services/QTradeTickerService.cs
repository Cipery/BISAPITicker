using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Entities;
using BISTickerAPI.Model;
using BISTickerAPI.Model.POCO;
using BISTickerAPI.Services.QTrade;

namespace BISTickerAPI.Services
{
    public class QTradeTickerService : ITicker
    {
        protected TickerDbContext dbContext;
        protected QTradeAPI qTradeApi;

        public QTradeTickerService(TickerDbContext dbContext, QTradeAPI qTradeApi)
        {
            this.dbContext = dbContext;
            this.qTradeApi = qTradeApi;
        }

        public bool UpdateTicker(string[] currencyPairs)
        {
            var data = qTradeApi.FetchMarkets();
            var exchange = dbContext.Exchanges.FirstOrDefault(e => e.Name.Equals(GetExchangeName()));
            var coins = dbContext.Coins.ToList();

            foreach (var pair in currencyPairs)
            {
                var split = pair.Split('/');
                var ticker = CreateTickerEntry(data, exchange,
                                coins.SingleOrDefault(c => c.Symbol.Equals(split[0])),
                                coins.SingleOrDefault(c => c.Symbol.Equals(split[1]))
                             );

                if (ticker == null)
                {
                    throw new Exception($"Ticker for pair {pair} got some issue!");
                }

                dbContext.Add(ticker);
            }

            dbContext.SaveChanges();

            return true;
        }

        protected TickerEntry CreateTickerEntry(List<QTradeTicker> tickers, Exchange exchange, Coin mainCoin, Coin baseCoin)
        {
            var tickerSymbol = $"{mainCoin.Symbol}_{baseCoin.Symbol}";
            var ticker = tickers.Find(t => t.IdLabel.Equals(tickerSymbol));
            if (ticker == null)
            {
                return null;
            }

            var tickerEntry = new TickerEntry()
            {
                AskPrice = ticker.Ask ?? Double.NaN,
                BidPrice = ticker.Bid ?? Double.NaN,
                Change = ticker.DayChange ?? Double.NaN,
                High = ticker.DayHigh ?? Double.NaN,
                Low = ticker.DayLow ?? Double.NaN,
                Volume = ticker.DayVolumeMarket ?? Double.NaN,
                BaseVolume = ticker.DayVolumeBase ?? Double.NaN,
                Open = ticker.DayOpen ?? Double.NaN,
                Label = mainCoin.Symbol,
                LastPrice = ticker.Last ?? Double.NaN,
                Timestamp = DateTime.UtcNow,
                Exchange = exchange,
                PairCoin1 = mainCoin,
                PairCoin2 = baseCoin
            };

            return tickerEntry;
        }

        public string GetExchangeName()
        {
            return "QTrade";
        }
    }
}
