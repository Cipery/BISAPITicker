﻿using System;
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
        protected TickerDbContext DbContext;
        protected QTradeApi QTradeApi;

        public QTradeTickerService(TickerDbContext dbContext, QTradeApi qTradeApi)
        {
            DbContext = dbContext;
            QTradeApi = qTradeApi;
        }

        public bool UpdateTicker(string[] currencyPairs)
        {
            var data = QTradeApi.FetchMarkets();
            var exchange = DbContext.Exchanges.FirstOrDefault(e => e.Name.Equals(GetExchangeName()));
            var coins = DbContext.Coins.ToList();

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
                    Console.WriteLine($"QTrade ticker does not have pair {pair}.");
#endif
                    continue;
                    //throw new Exception($"Ticker for pair {pair} got some issue!");
                }

                DbContext.Add(ticker);
            }

            DbContext.SaveChanges();

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
                AskPrice = ticker.Ask ?? 0,
                BidPrice = ticker.Bid ?? 0,
                Change = ticker.DayChange,
                High = ticker.DayHigh,
                Low = ticker.DayLow,
                Volume = ticker.DayVolumeMarket,
                BaseVolume = ticker.DayVolumeBase,
                Open = ticker.DayOpen,
                Label = mainCoin.Symbol,
                LastPrice = ticker.Last ?? 0,
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
