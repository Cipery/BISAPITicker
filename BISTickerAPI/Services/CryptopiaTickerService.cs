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
        protected TickerDbContext dbContext;
        protected ICryptopiaAPI cryptopiaAPI;

        public CryptopiaTickerService(TickerDbContext dbContext, ICryptopiaAPI cryptopiaAPI)
        {
            this.dbContext = dbContext;
            this.cryptopiaAPI = cryptopiaAPI;
        }

        public bool UpdateTicker(string[] currencyPairs)
        {
            foreach(var pair in currencyPairs)
            {
                var split = pair.Split('/');
                var ticker = FetchTickerData(split[0], split[1]);

                if(ticker == null)
                {
                    throw new Exception($"Ticker for pair {pair} got some issue!");
                }

                dbContext.Add(ticker);
            }

            dbContext.SaveChanges();

            return true;
        }

        public string GetExchangeName()
        {
            return "Cryptopia";
        }

        public object GetCoinInfo(string leftCoin, string rightCoin)
        {
            return null;
        }

        /*public object GetCoinInfoJsonCached(string leftCoin, string rightCoin)
        {
            var cached = cacheService.GetCachedObject(GenerateCacheKeyForPair(leftCoin, rightCoin));

            if (cached != null)
            {
                return cached;
            }

            cached = GetCoinInfo(leftCoin, rightCoin);

            if (cached == null)
            {
                return null;
            }

            cached = JsonConvert.SerializeObject(cached, Formatting.Indented);
            cacheService.AddCachedObject(GenerateCacheKeyForPair(leftCoin, rightCoin), cached);

            return cached;
        }*/

        public TickerEntry FetchTickerData(string leftCoin, string rightCoin)
        {
            var cryptopiaMarkets = cryptopiaAPI.FetchMarkets(rightCoin);
            var coins = dbContext.Coins;
            var exchange = dbContext.Exchanges.Single(exch => exch.Name.Equals(GetExchangeName()));
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
            var coin = dbContext.Coins.Single(c => c.Symbol == leftCoin);
            var baseCoin = dbContext.Coins.Single(c => c.Symbol == rightCoin);
            return (coin, baseCoin);
        }
    }
}
