using BISTickerAPI.Entities;
using BISTickerAPI.Helpers;
using BISTickerAPI.Model;
using BISTickerAPI.Model.POCO;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace BISTickerAPI.Services
{
    public class CryptopiaTickerService : ITicker
    {
        protected TickerDbContext DbContext;
        protected ICryptopiaApi CryptopiaApi;

        public CryptopiaTickerService(TickerDbContext dbContext, ICryptopiaApi cryptopiaAPI)
        {
            this.DbContext = dbContext;
            this.CryptopiaApi = cryptopiaAPI;
        }

        public bool UpdateTicker(string[] currencyPairs)
        {
            foreach(var pair in currencyPairs)
            {
                var split = pair.Split('/');
                var ticker = FetchTickerData(split[0], split[1]);

                if(ticker == null)
                {
                    Console.WriteLine($"Cryptopia ticker does not have pair {pair}.");
                    continue;
                    //throw new Exception($"Ticker for pair {pair} got some issue!");
                }

                DbContext.Add(ticker);
            }

            DbContext.SaveChanges();

            return true;
        }

        public string GetExchangeName()
        {
            return "Cryptopia";
        }

        public TickerEntry FetchTickerData(string leftCoin, string rightCoin)
        {
            var cryptopiaMarkets = CryptopiaApi.FetchMarkets(rightCoin);
            var coins = DbContext.Coins.ToList();
            var exchange = DbContext.Exchanges.Single(exch => exch.Name.Equals(GetExchangeName()));
            // JSON convert fills almost all data needed. However, we still need to fill some stuff
            cryptopiaMarkets.Data.ForEach(p =>
            {
                var coinLabel = p.Label.Split("/")[0];
                p.PairCoin2 = coins.SingleOrDefault(c => c.Symbol == rightCoin);
                p.PairCoin1 = coins.SingleOrDefault(c => c.Symbol == coinLabel);
                p.Timestamp = DateTime.UtcNow;
                p.Exchange = exchange;
            });

            var tradingPairJoined = $"{leftCoin}/{rightCoin}";
            return cryptopiaMarkets.Data.First(entry => entry.Label ==  tradingPairJoined);
        }

        public (Coin, Coin) GetCoins(string leftCoin, string rightCoin)
        {
            var coin = DbContext.Coins.Single(c => c.Symbol == leftCoin);
            var baseCoin = DbContext.Coins.Single(c => c.Symbol == rightCoin);
            return (coin, baseCoin);
        }
    }
}
