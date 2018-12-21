using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace BISTickerAPI.Services.TradeSatoshi
{
    public class TradeSatoshiRestClient
    {
        public virtual IRestClient RestClient { get; set; } = new RestClient("https://tradesatoshi.com/api");
    }
}
