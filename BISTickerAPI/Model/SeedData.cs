using BISTickerAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Model
{
    public class SeedData
    {
        public static void Initialize(TickerDbContext dbContext, Coin[] coins)
        {
            foreach (var coin1 in coins.Where(p => !dbContext.Coins.Any(coin => coin.Symbol.Equals(p.Symbol))))
            {
                dbContext.Coins.Add(coin1);
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
