using RestSharp;

namespace BISTickerAPI.Services.QTrade
{
    public class QTradeRestClient
    {
        public virtual IRestClient RestClient { get; set; } = new RestClient("https://api.qtrade.io/v1");
    }
}