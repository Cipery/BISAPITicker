using BISTickerAPI.Model;
using BISTickerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestSharp;
using System;
using Xunit;
using System.Linq;
using System.IO;
using BISTickerAPI.Helpers;
using BISTickerAPI.Services.QTrade;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BISTickerAPI.Tests
{
    public class TickerServiceTest
    {
        private const string MEMORY_COMMON_DB_NAME = "BAKARYUUHA";

        #region SETUP
        public static IOptions<AppSettings> MockAppSettings()
        {
            var mock = new Mock<AppSettings>();
            mock.Setup(call => call.FetchPairs).Returns(new string[] { "BTC/USDT", "BIS/BTC" });

            var mockOptions = new Mock<IOptions<AppSettings>>();
            mockOptions.Setup(call => call.Value).Returns(mock.Object);
            return mockOptions.Object;
        }

        public static string ReadFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader st = new StreamReader(fs))
                {
                    return st.ReadToEnd();
                }
            }
        }

        public static TickerDbContext CreateDbContext(string dbName = null, bool clearData = false)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<TickerDbContext>().UseInMemoryDatabase(dbName == null ? Guid.NewGuid().ToString() : dbName);
            var dbContext = new TickerDbContext(dbOptionsBuilder.Options);
            if (clearData)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
            else
            {
                //dbContext.Database.Migrate();
            }
            SeedData.Initialize(dbContext);
            return dbContext;
        }

        public static ICacheService MockCacheService()
        {
            var mock = new Mock<ICacheService>();
            mock.Setup(call => call.GetCachedObject(It.IsAny<string>())).Returns(null);
            return mock.Object;
        }


        public static ICryptopiaApi MockCryptopiaAPI()
        {

            var mockResponseBTC = new Mock<IRestResponse>();
            mockResponseBTC.Setup(call => call.IsSuccessful).Returns(true);
            mockResponseBTC.Setup(call => call.Content).Returns(ReadFile("./market_btc.json"));
            var mockResponseUSDT = new Mock<IRestResponse>();
            mockResponseUSDT.Setup(call => call.IsSuccessful).Returns(true);
            mockResponseUSDT.Setup(call => call.Content).Returns(ReadFile("./market_usdt.json"));
            var mockRestClient = new Mock<IRestClient>();

            mockRestClient.Setup(call => call.Execute(It.IsAny<IRestRequest>()))
                .Returns((IRestRequest req) => (string)req.Parameters.First(p => p.Name == "baseCoinSymbol").Value == "BTC" ? mockResponseBTC.Object : mockResponseUSDT.Object);

            var cryptopiaApi = new CryptopiaAPI(mockRestClient.Object);
            return cryptopiaApi;
        }

        public static QTradeApi MockQTradeApi()
        {
            var mockResponse = new Mock<IRestResponse>();
            mockResponse.Setup(call => call.IsSuccessful).Returns(true);
            mockResponse.Setup(call => call.Content).Returns(ReadFile("./qtrade_tickers.json"));

            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(call => call.Execute(It.IsAny<IRestRequest>()))
                .Returns(mockResponse.Object);

            var mockQTradeRestClient = new Mock<QTradeRestClient>();
            mockQTradeRestClient.Setup(call => call.RestClient).Returns(mockRestClient.Object);
            
            return new QTradeApi(mockQTradeRestClient.Object);
        }

        #endregion

        #region Cryptopia

        [Fact]
        public void TestFetchingFromCryptopiaFile()
        {
            using (var db = CreateDbContext(MEMORY_COMMON_DB_NAME))
            {
                var cryptopiaMockApi = MockCryptopiaAPI();
                var tickerService = new CryptopiaTickerService(db, cryptopiaMockApi);
                var result = tickerService.FetchTickerData("BIS", "BTC");
                Assert.NotNull(result);
                Assert.Equal("BIS/BTC", result.Label);
                Assert.True(result.LastPrice > 0);
                Assert.True(result.PairCoin1 != null && result.PairCoin1.Symbol == "BIS");
                Assert.True(result.PairCoin2 != null && result.PairCoin2.Symbol == "BTC");
            }
        }

        [Theory]
        [InlineData("BIS", "BTC")]
        public void TestGetCoinsFromCryptopiaTicker(string coin1, string coin2)
        {
            using (var db = CreateDbContext(MEMORY_COMMON_DB_NAME))
            {
                var cryptopiaMockApi = MockCryptopiaAPI();
                var tickerService = new CryptopiaTickerService(db, cryptopiaMockApi);
                var coins = tickerService.GetCoins(coin1, coin2);
                Assert.Equal(coin1, coins.Item1.Symbol);
                Assert.Equal(coin2, coins.Item2.Symbol);
            }
        }

        [Fact]
        public void TestUpdateCryptopiaTicker()
        {
            using (var db = CreateDbContext(MEMORY_COMMON_DB_NAME))
            {
                var cryptopiaMockApi = MockCryptopiaAPI();
                var tickerService = new CryptopiaTickerService(db, cryptopiaMockApi);
                Assert.True(!db.TickerEntries.Any());
                Assert.True(tickerService.UpdateTicker(new[] { "BTC/USDT" }));
                Assert.True(db.TickerEntries.Any());
            }
        }

        [Theory]
        [InlineData("BIS", "BTC")]
        [InlineData("BTC", "USDT")]
        [Trait("Category", "RealAPI")]
        public void TestFetchingFromRealCryptopiaApi(string coinSymbol, string baseCoinSymbol)
        {
            using (var db = CreateDbContext(MEMORY_COMMON_DB_NAME))
            {
                var cryptopiaApi = new CryptopiaAPI(new RestClient("https://www.cryptopia.co.nz/api"));
                var tickerService = new CryptopiaTickerService(db, cryptopiaApi);
                var result = tickerService.FetchTickerData(coinSymbol, baseCoinSymbol);
                Assert.NotNull(result);
                Assert.Equal($"{coinSymbol}/{baseCoinSymbol}", result.Label);
                Assert.True(result.LastPrice > 0);
            }
        }

        #endregion

        #region QTrade

        [Fact]
        public void TestFetchingFromQTradeFile()
        {
            var mockApi = MockQTradeApi();
            var markets = mockApi.FetchMarkets();

            Assert.True(markets.Any());
            Assert.Contains(markets, p => p.IdLabel == "BIS_BTC");
        }

        [Theory]
        [InlineData("BIS","BTC")]
        public void TestQTradeTickerServiceFileFetch(string coin1, string coin2)
        {
            using (var dbContext = CreateDbContext())
            {
                var mockApi = MockQTradeApi();
                var qTradeTickerService = new QTradeTickerService(dbContext, mockApi);
                Assert.True(qTradeTickerService.UpdateTicker(new[] {$"{coin1}/{coin2}"}));
                Assert.True(dbContext.TickerEntries.Any());
                Assert.True(dbContext.TickerEntries.Any(entry => entry.PairCoin1.Symbol.Equals(coin1)));
            }
        }

        [Theory]
        [InlineData("BIS", "BTC")]
        public void TestQTradeTickerServiceRealFetch(string coin1, string coin2)
        {
            using (var dbContext = CreateDbContext())
            {
                var qTradeApi = new QTradeApi(new QTradeRestClient());
                var qTradeTickerService = new QTradeTickerService(dbContext, qTradeApi);
                Assert.True(qTradeTickerService.UpdateTicker(new[] { $"{coin1}/{coin2}" }));
                Assert.True(dbContext.TickerEntries.Any());
                Assert.True(dbContext.TickerEntries.Any(entry => entry.PairCoin1.Symbol.Equals(coin1)));
            }
        }

        #endregion

        #region Aggregator

        /// <summary>
        /// I need to do some more important things right now (read as: I am feeling super lazy)
        /// so I
        /// </summary>
        [Fact]
        public void TestGetCoins()
        {
            var dbContext = CreateDbContext();
            var cryptopiaTicker = new CryptopiaTickerService(dbContext, MockCryptopiaAPI());
            var qTradeTicker = new QTradeTickerService(dbContext, MockQTradeApi());
            var aggregatorService = new AggregatorService(CreateDbContext(), MockAppSettings(), null, cryptopiaTicker, qTradeTicker);
            var coins = dbContext.GetCoins("BIS", "BTC");
            Assert.NotNull(coins.Item1);
            Assert.NotNull(coins.Item2);
            Assert.Equal("BIS", coins.Item1.Symbol);
            Assert.Equal("BTC", coins.Item2.Symbol);
        }

        #endregion
    }
}
