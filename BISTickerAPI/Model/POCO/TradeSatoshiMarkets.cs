using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BISTickerAPI.Model.POCO
{
    public class TradeSatoshiMarkets
    {
        [JsonProperty(PropertyName = "market")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "last")]
        public double Last { get; set; }
        [JsonProperty(PropertyName = "ask")]
        public double Ask { get; set; }
        [JsonProperty(PropertyName = "bid")]
        public double Bid { get; set; }
        [JsonProperty(PropertyName = "change")]
        public double? Change { get; set; }
        [JsonProperty(PropertyName = "high")]
        public double? High { get; set; }
        [JsonProperty(PropertyName = "low")]
        public double? Low { get; set; }
        [JsonProperty(PropertyName = "baseVolume")]
        public double? VolumeBase { get; set; }
        [JsonProperty(PropertyName = "volume")]
        public double? Volume { get; set; }
    }
}
