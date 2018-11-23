using BISTickerAPI.Model.POCO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Services
{
    public class CryptopiaAPI : ICryptopiaApi
    {
        protected IRestClient CryptopiaClient;

        public CryptopiaAPI(IRestClient cryptopiaClient)
        {
            this.CryptopiaClient = cryptopiaClient;
        }

        public CryptopiaMarkets FetchMarkets(string baseCoinSymbol)
        {
            var request = new RestRequest("GetMarkets/{baseCoinSymbol}");
            request.AddParameter("baseCoinSymbol", baseCoinSymbol, ParameterType.UrlSegment);
            var restResponse = CryptopiaClient.Execute(request);
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
