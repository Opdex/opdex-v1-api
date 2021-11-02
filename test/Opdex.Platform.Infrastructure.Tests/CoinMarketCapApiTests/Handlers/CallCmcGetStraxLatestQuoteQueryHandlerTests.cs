using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinMarketCapApiTests.Handlers
{
    public class CallCmcGetStraxLatestQuoteQueryHandlerTests
    {
        private readonly Mock<IQuotesModule> _quotesModule;
        private readonly CallCmcGetStraxLatestQuoteQueryHandler _handler;

        public CallCmcGetStraxLatestQuoteQueryHandlerTests()
        {
            _quotesModule = new Mock<IQuotesModule>();
            var logger = NullLogger<CallCmcGetStraxLatestQuoteQueryHandler>.Instance;
            _handler = new CallCmcGetStraxLatestQuoteQueryHandler(_quotesModule.Object, logger);
        }

        [Fact]
        public async Task CallCmcGetStraxLatestQuoteQuery_Sends_GetLatestQuoteAsync()
        {
            // Arrange
            // Act
            try
            {
                await _handler.Handle(new CallCmcGetStraxLatestQuoteQuery(), CancellationToken.None);
            }
            catch { }

            // Assert
            _quotesModule.Verify(callTo => callTo.GetLatestQuoteAsync(CmcTokens.STRAX, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CallCmcGetStraxLatestQuoteQuery_ReturnsZero_InvalidQuote()
        {
            // Arrange
            _quotesModule.Setup(callTo => callTo.GetLatestQuoteAsync(CmcTokens.STRAX, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var response = await _handler.Handle(new CallCmcGetStraxLatestQuoteQuery(), CancellationToken.None);

            // Assert
            response.Should().Be(0m);
        }

        [Fact]
        public async Task CallCmcGetStraxLatestQuoteQuery_ReturnsZero_UsdPriceNull()
        {
            // Arrange
            var quote = new CmcLatestQuote { Data = new Dictionary<string, LatestQuoteToken> { { CmcTokens.STRAX.ToString(), new LatestQuoteToken
            {
                Quote = new Dictionary<string, LatestQuotePrice>()
            } } } };

            _quotesModule.Setup(callTo => callTo.GetLatestQuoteAsync(CmcTokens.STRAX, CancellationToken.None))
                .ReturnsAsync(quote);

            // Act
            var response = await _handler.Handle(new CallCmcGetStraxLatestQuoteQuery(), CancellationToken.None);

            // Assert
            response.Should().Be(0m);
        }

        [Fact]
        public async Task CallCmcGetStraxLatestQuoteQuery_ReturnsPrice()
        {
            // Arrange
            const decimal price = 1.1m;
            var quote = new CmcLatestQuote { Data = new Dictionary<string, LatestQuoteToken> { { CmcTokens.STRAX.ToString(), new LatestQuoteToken
            {
                Quote = new Dictionary<string, LatestQuotePrice> { {"USD", new LatestQuotePrice { Price = price }}  }
            } } } };

            _quotesModule.Setup(callTo => callTo.GetLatestQuoteAsync(CmcTokens.STRAX, CancellationToken.None))
                .ReturnsAsync(quote);

            // Act
            var response = await _handler.Handle(new CallCmcGetStraxLatestQuoteQuery(), CancellationToken.None);

            // Assert
            response.Should().Be(price);
        }
    }
}
