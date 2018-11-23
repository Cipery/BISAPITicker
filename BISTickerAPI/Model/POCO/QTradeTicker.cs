using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BISTickerAPI.Model.POCO
{
    public class QTradeTicker
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "id_hr")]
        public string IdLabel { get; set; }
        public double? Last { get; set; }
        public double? Ask { get; set; }
        public double? Bid { get; set; }
        [JsonProperty(PropertyName = "day_avg_price")]
        public double? DayAvgPrice { get; set; }
        [JsonProperty(PropertyName = "day_change")]
        public double? DayChange { get; set; }
        [JsonProperty(PropertyName = "day_high")]
        public double? DayHigh { get; set; }
        [JsonProperty(PropertyName = "day_low")]
        public double? DayLow { get; set; }
        [JsonProperty(PropertyName = "day_open")]
        public double? DayOpen { get; set; }
        [JsonProperty(PropertyName = "day_volume_base")]
        public double? DayVolumeBase { get; set; }
        [JsonProperty(PropertyName = "day_volume_market")]
        public double? DayVolumeMarket { get; set; }
    }
}
