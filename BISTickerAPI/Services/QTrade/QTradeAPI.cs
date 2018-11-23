using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Model.POCO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BISTickerAPI.Services.QTrade
{
    public class QTradeApi
    {
        protected IRestClient RestClient;

        public QTradeApi(QTradeRestClient qTradeRestClient)
        {
            RestClient = qTradeRestClient.RestClient;
        }


        public virtual List<QTradeTicker> FetchMarkets()
        {
            var request = new RestRequest("/tickers");
            var restResponse = RestClient.Execute(request);

            if (!restResponse.IsSuccessful)
            {
                //TODO: unknown host ?
                throw new Exception($"Request exception! Error message: {restResponse.ErrorMessage}");
            }

            var jObject = JObject.Parse(restResponse.Content);

            return jObject["data"]["markets"].Select(token => token.ToObject<QTradeTicker>()).ToList();
        }
    }
}
