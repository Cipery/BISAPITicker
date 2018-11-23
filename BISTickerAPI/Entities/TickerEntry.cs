using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Entities
{
    public class TickerEntry
    {
        public long Id { get; set; }
        public Coin PairCoin1 { get; set; }
        public Coin PairCoin2 { get; set; }
        public Exchange Exchange { get; set; }
        public DateTime Timestamp { get; set; }

        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
        public double LastPrice { get; set; }

        public double? Low { get; set; }
        public double? High { get; set; }
        public double? Volume { get; set; } 
        public double? SellVolume { get; set; } 
        public double? BuyVolume { get; set; } 
        public double? Change { get; set; } 
        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? BaseVolume { get; set; } 
        public double? BuyBaseVolume { get; set; }
        public double? SellBaseVolume { get; set; } 

        [NotMapped]
        public string Label { get; set; }
    }
}
