using BISTickerAPI.Model.POCO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Services
{
    public class CryptopiaAPI : ICryptopiaAPI
    {
        protected IRestClient cryptopiaClient;

        public CryptopiaAPI(IRestClient cryptopiaClient)
        {
            this.cryptopiaClient = cryptopiaClient;
        }

        public CryptopiaMarkets FetchMarkets(string baseCoinSymbol)
        {
            var request = new RestRequest("GetMarkets/{baseCoinSymbol}");
            request.AddParameter("baseCoinSymbol", baseCoinSymbol, ParameterType.UrlSegment);
            var restResponse = cryptopiaClient.Execute(request);
            if (!restResponse.IsSuccessful)
            {
                //TODO: unknown host
                throw new Exception($"Request exception! Error message: {restResponse.ErrorMessage}");
            }
            var deserializedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<CryptopiaMarkets>(restResponse.Content);

            return deserializedObject;
        }
    }
}
