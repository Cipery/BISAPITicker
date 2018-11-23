using BISTickerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
