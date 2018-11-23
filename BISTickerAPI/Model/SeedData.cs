using BISTickerAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Model
{
    public class SeedData
    {
        public static void Initialize(TickerDbContext dbContext)
        {
            if(!dbContext.Coins.Any())
            {
                dbContext.Add(new Coin() { Name = "Bismuth", Symbol = "BIS" });
                dbContext.Add(new Coin() { Name = "Bitcoin", Symbol = "BTC" });
                dbContext.Add(new Coin() { Name = "Litecoin", Symbol = "LTC" });
                dbContext.Add(new Coin() { Name = "Dogecoin", Symbol = "DOGE" });
                dbContext.Add(new Coin() { Name = "USDTether", Symbol = "USDT" });
            }

            if(!dbContext.Exchanges.Any(exc => exc.Name.Equals("Cryptopia")))
            {
                dbContext.Add(new Exchange() { Name = "Cryptopia"});
            }

            if (!dbContext.Exchanges.Any(exc => exc.Name.Equals("QTrade")))
            {
                dbContext.Add(new Exchange() { Name = "QTrade" });
            }

            dbContext.SaveChanges();
        }
    }
}
