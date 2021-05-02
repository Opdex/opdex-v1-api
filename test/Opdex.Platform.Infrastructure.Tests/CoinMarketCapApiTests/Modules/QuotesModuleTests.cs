using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinMarketCapApiTests.Modules
{
    public class QuotesModuleTests
    {
        private readonly IQuotesModule _quotesModule;

        public QuotesModuleTests()
        {
            var configuration = new CoinMarketCapConfiguration
            {   
                ApiKey = "19ba87c8-f1b8-4228-826c-47d671f1f6ab",
                ApiUrl = "https://pro-api.coinmarketcap.com/v2/cryptocurrency/",
                HeaderName = "X-CMC_PRO_API_KEY"
            };
            
            var httpClient = new HttpClient(new HttpClientHandler());
            httpClient.BuildHttpClient(configuration);
            
            _quotesModule = new QuotesModule(httpClient, NullLogger<QuotesModule>.Instance);
        }

        [Fact]
        public async Task ReceiptSearch_Success()
        {
            // arrange
            const int straxTokenId = 1343;
            
            // act
            var response = await _quotesModule.GetQuoteAsync(straxTokenId, CancellationToken.None);
            
            // assert
            response.Should().NotBeNull();
        }
    }
}