using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Contrib.HttpClient;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinMarketCapApiTests.Modules
{
    public class QuotesModuleTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly IQuotesModule _quotesModule;

        private const string BaseAddress = "https://pro-api.coinmarketcap.com/v2/cryptocurrency/";

        public QuotesModuleTests()
        {
            _handler = new Mock<HttpMessageHandler>();
            var logger = NullLogger<QuotesModule>.Instance;

            var httpClient = _handler.CreateClient();
            httpClient.BaseAddress = new Uri(BaseAddress);

            _quotesModule = new QuotesModule(httpClient, logger);
        }

        [Fact]
        public async Task LatestQuote_SendRequest()
        {
            // arrange
            const int tokenId = CmcTokens.STRAX;

            _handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");

            // act
            await _quotesModule.GetLatestQuoteAsync(tokenId, CancellationToken.None);

            // assert
            _handler.VerifyRequest(HttpMethod.Get, $"{BaseAddress}quotes/latest?id={tokenId}");
        }

        [Fact]
        public async Task HistoricalQuote_SendRequest()
        {
            // arrange
            const int tokenId = CmcTokens.STRAX;
            var time = DateTime.UtcNow;

            _handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");

            // act
            await _quotesModule.GetHistoricalQuoteAsync(tokenId, time, CancellationToken.None);

            // assert
            _handler.VerifyRequest(HttpMethod.Get, $"{BaseAddress}quotes/historical?id={tokenId}&time_start={time}&interval=5m&count=1");
        }
    }
}
