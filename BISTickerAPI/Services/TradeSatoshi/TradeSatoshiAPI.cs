using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Model.POCO;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BISTickerAPI.Services.TradeSatoshi
{
    public class TradeSatoshiAPI
    {
        private IRestClient _restClient;

        public TradeSatoshiAPI(TradeSatoshiRestClient restClient)
        {
            _restClient = restClient.RestClient;
        }

        public virtual List<TradeSatoshiMarkets> FetchMarkets()
        {
            var request = new RestRequest("/public/getmarketsummaries");
            var restResponse = _restClient.Execute(request);

            if (!restResponse.IsSuccessful)
            {
                //TODO: unknown host ?
                throw new Exception($"Request exception! Error message: {restResponse.ErrorMessage}");
            }

            var jObject = JObject.Parse(restResponse.Content);

            if (!jObject.HasValues || !jObject.ContainsKey("success") || !(bool) jObject["success"])
            {
                throw new Exception($"Request exception! Error message: {jObject}");
            }

            var sadongaTanes = jObject["result"].Select(token => token.ToObject<TradeSatoshiMarkets>()).ToList();

            return jObject["result"].Select(token => token.ToObject<TradeSatoshiMarkets>()).ToList();
        }
    }
}
