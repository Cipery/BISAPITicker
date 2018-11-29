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
        // Edit: Should be fixed by 29.11.2018 commits - AntiDbDoSAggregatorService
        // because people could just brute spam with random coin symbols, and since each request will hit database
        // 
        // Note: There should never be any uncached endpoint. 

        // GET: api/price
        [HttpGet("price/{mainCoin?}/{baseCoin?}")]
        [Produces("application/json")]
        public ContentResult GetPrice(string mainCoin = "BIS", string baseCoin = "BTC")
        {
            return Content((string)AggregatorService.GetAveragedOuput(mainCoin, baseCoin));
        }

        // GET: api/price
        [HttpGet("markets/{mainCoin?}/{baseCoin?}")]
        [Produces("application/json")]
        public ContentResult GetMarkets(string mainCoin = "BIS", string baseCoin = "BTC")
        {
            return Content((string)AggregatorService.GetPerExchangeOutput(mainCoin, baseCoin));
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
