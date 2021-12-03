using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi;
using Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinMarketCapApiTests.Handlers;

public class CallCmcGetStraxHistoricalQuoteQueryHandlerTests
{
    private readonly Mock<IQuotesModule> _quotesModule;
    private readonly Mock<ILogger<CallCmcGetStraxHistoricalQuoteQueryHandler>> _logger;
    private readonly CallCmcGetStraxHistoricalQuoteQueryHandler _handler;

    public CallCmcGetStraxHistoricalQuoteQueryHandlerTests()
    {
        _quotesModule = new Mock<IQuotesModule>();
        _logger = new Mock<ILogger<CallCmcGetStraxHistoricalQuoteQueryHandler>>();
        _handler = new CallCmcGetStraxHistoricalQuoteQueryHandler(_quotesModule.Object, _logger.Object);
    }

    [Fact]
    public void CallCmcGetStraxHistoricalQuoteQuery_InvalidDateTime_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new CallCmcGetStraxHistoricalQuoteQuery(default);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("CMC quote datetime must be set.");
    }

    [Fact]
    public async Task CallCmcGetStraxHistoricalQuoteQuery_Sends_GetHistoricalQuoteAsync()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;

        // Act
        try
        {
            await _handler.Handle(new CallCmcGetStraxHistoricalQuoteQuery(dateTime), CancellationToken.None);
        }
        catch { }

        // Assert
        _quotesModule.Verify(callTo => callTo.GetHistoricalQuoteAsync(CmcTokens.STRAX, dateTime, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CallCmcGetStraxHistoricalQuoteQuery_ReturnsZero_FiatPricesAreNull()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;

        _quotesModule.Setup(callTo => callTo.GetHistoricalQuoteAsync(CmcTokens.STRAX, dateTime, CancellationToken.None))
            .ReturnsAsync(() => null);

        // Act
        var response = await _handler.Handle(new CallCmcGetStraxHistoricalQuoteQuery(dateTime), CancellationToken.None);

        // Assert
        response.Should().Be(0m);
    }

    [Fact]
    public async Task CallCmcGetStraxHistoricalQuoteQuery_ReturnsZero_UsdPriceIsNull()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;
        var quote = new CmcHistoricalQuote
        {
            Data = new HistoricalQuoteData
            {
                Quotes = new List<HistoricalQuoteTimeframe>
                {
                    new HistoricalQuoteTimeframe { Quote = new Dictionary<string, HistoricalQuotePrice>() }
                }
            }
        };

        _quotesModule.Setup(callTo => callTo.GetHistoricalQuoteAsync(CmcTokens.STRAX, dateTime, CancellationToken.None))
            .ReturnsAsync(quote);

        // Act
        var response = await _handler.Handle(new CallCmcGetStraxHistoricalQuoteQuery(dateTime), CancellationToken.None);

        // Assert
        response.Should().Be(0m);
    }

    [Fact]
    public async Task CallCmcGetStraxHistoricalQuoteQuery_ReturnsPrice()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;
        const decimal price = 1.1m;

        var quote = new CmcHistoricalQuote
        {
            Data = new HistoricalQuoteData
            {
                Quotes = new List<HistoricalQuoteTimeframe>
                {
                    new HistoricalQuoteTimeframe
                    {
                        Quote = new Dictionary<string, HistoricalQuotePrice> { { "USD", new HistoricalQuotePrice { Price = price } } }
                    }
                }
            }
        };

        _quotesModule.Setup(callTo => callTo.GetHistoricalQuoteAsync(CmcTokens.STRAX, dateTime, CancellationToken.None))
            .ReturnsAsync(quote);

        // Act
        var response = await _handler.Handle(new CallCmcGetStraxHistoricalQuoteQuery(dateTime), CancellationToken.None);

        // Assert
        response.Should().Be(price);
    }
}