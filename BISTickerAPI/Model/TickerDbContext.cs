using BISTickerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Model.Exceptions;

namespace BISTickerAPI.Model
{
    public class TickerDbContext : DbContext
    {
        public DbSet<TickerEntry> TickerEntries { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }

        public TickerDbContext(DbContextOptions<TickerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="baseCoin"></param>
        /// <returns></returns>
        public TickerEntry GetTickerEntry(Coin coin, Coin baseCoin)
        {
            return TickerEntries.Where(t => t.PairCoin1 == coin && t.PairCoin2 == baseCoin)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
        }

        public TickerEntry GetTickerEntry(Coin coin, Coin baseCoin, Exchange exchange)
        {
            return TickerEntries.Where(t => t.PairCoin1 == coin && t.PairCoin2 == baseCoin && t.Exchange == exchange)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
        }

        public Coin GetCoin(string coinSymbol)
        {
            var coin = Coins.SingleOrDefault(c => c.Symbol == coinSymbol);
            if (coin == null)
            {
                throw new CoinNotFoundException { CoinSymbol = coinSymbol };
            }

            return coin;
        }

        public (Coin, Coin) GetCoins(string leftCoin, string rightCoin)
        {
            var coin = GetCoin(leftCoin);
            var baseCoin = GetCoin(rightCoin);

            return (coin, baseCoin);
        }
    }
}
