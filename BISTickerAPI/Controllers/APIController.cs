using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Services;
using BISTickerAPI.Services.QTrade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BISTickerAPI.Controllers
{
    [Route("api/")]
    public class ApiController : Controller
    {
        protected MemoryCachingAggregatorService AggregatorService;

        public ApiController(MemoryCachingAggregatorService aggregatorService)
        {
            AggregatorService = aggregatorService;
        }

        // TODO: There should be some in-memory cache with available coin keys (maybe just cache coins entities)
        // because people could just brute spam with random coin symbols, and since each request will hit database
        // 
        // Note: There should never be any uncached endpoint. 

        // GET: api/price
        [HttpGet("price/{mainCoin?}/{baseCoin?}")]
        [Produces("application/json")]
        public ContentResult GetPrice(string mainCoin = "BIS", string baseCoin = "BTC")
        {
            Console.WriteLine("Api has been accessed");
            var result = AggregatorService.GetAveragedOuput(mainCoin, baseCoin);
            //AggregatorService.Uda
            return Content((string)result);
        }

        // GET: api/price
        [HttpGet("markets/{mainCoin?}/{baseCoin?}")]
        [Produces("application/json")]
        public ContentResult GetMarkets(string mainCoin = "BIS", string baseCoin = "BTC")
        {
            var result = AggregatorService.GetPerExchangeOutput(mainCoin, baseCoin);
            return Content((string) result);
        }

        // GET: api/test
        [HttpGet("test")]
        [Produces("application/json")]
        public ContentResult TestAction([FromServices] QTradeTickerService tickerService)
        {
            return Content("");
        }
    }
}
