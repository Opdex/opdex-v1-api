using FluentAssertions;
using MediatR;
using Microsoft.FeatureManagement;
using Moq;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Feeds;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Feeds;

public class FiatPriceFeedTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IFeatureManager> _featureManagerMock;

    private readonly FiatPriceFeed _fiatPriceFeed;

    public FiatPriceFeedTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _featureManagerMock = new Mock<IFeatureManager>();
        _fiatPriceFeed = new FiatPriceFeed(_mediatorMock.Object, _featureManagerMock.Object);
    }

    [Fact]
    public async Task GetCrsUsdPrice_Within5MinsCmcFeatureDisabled_CallCoinGeckoGetStraxLatestPriceQuery()
    {
        // Arrange
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
                           .ReturnsAsync(false);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxLatestPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(DateTime.UtcNow.AddMinutes(-4), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxLatestPriceQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        price.Should().Be(10.00m);
    }

    [Fact]
    public async Task GetCrsUsdPrice_LaterThan5MinsCmcFeatureDisabled_CallCoinGeckoGetStraxHistoricalPriceQuery()
    {
        // Arrange
        var blockTime = DateTime.UtcNow.AddMinutes(-5); // accounting for execution time, this will be >5 mins
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
                           .ReturnsAsync(false);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxHistoricalPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(blockTime, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxHistoricalQuoteQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCoinGeckoGetStraxHistoricalPriceQuery>(q => q.DateTime == blockTime), It.IsAny<CancellationToken>()), Times.Once);
        price.Should().Be(1.00m);
    }

    [Fact]
    public async Task GetCrsUsdPrice_Within5MinsCmcFeatureEnabledReturnsPrice_CallCmcGetStraxLatestPriceQuery()
    {
        // Arrange
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
                           .ReturnsAsync(true);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(5.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(DateTime.UtcNow.AddMinutes(-4), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxLatestPriceQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        price.Should().Be(5.00m);
    }

    [Fact]
    public async Task GetCrsUsdPrice_Within5MinsCmcFeatureEnabledReturnsZero_FallbackToCoinGecko()
    {
        // Arrange
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
            .ReturnsAsync(true);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.00m);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxLatestPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(DateTime.UtcNow.AddMinutes(-4), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxLatestPriceQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        price.Should().Be(10.00m);
    }

    [Fact]
    public async Task GetCrsUsdPrice_LaterThan5MinsCmcFeatureEnabledReturnsPrice_CallCmcGetStraxHistoricalQuoteQuery()
    {
        // Arrange
        var blockTime = DateTime.UtcNow.AddMinutes(-5); // accounting for execution time, this will be >5 mins
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
                           .ReturnsAsync(true);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCmcGetStraxHistoricalQuoteQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(5.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(blockTime, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCmcGetStraxHistoricalQuoteQuery>(q => q.DateTime == blockTime), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxHistoricalPriceQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        price.Should().Be(5.00m);
    }

    [Fact]
    public async Task GetCrsUsdPrice_LaterThan5MinsCmcFeatureEnabledReturnsZero_FallbackToCoinGecko()
    {
        // Arrange
        var blockTime = DateTime.UtcNow.AddMinutes(-5); // accounting for execution time, this will be >5 mins
        _featureManagerMock.Setup(callTo => callTo.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
            .ReturnsAsync(true);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCmcGetStraxHistoricalQuoteQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0.00m);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCoinGeckoGetStraxHistoricalPriceQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10.00m);

        // Act
        var price = await _fiatPriceFeed.GetCrsUsdPrice(blockTime, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCmcGetStraxHistoricalQuoteQuery>(q => q.DateTime == blockTime), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCoinGeckoGetStraxHistoricalPriceQuery>(q => q.DateTime == blockTime), It.IsAny<CancellationToken>()), Times.Once);
        price.Should().Be(10.00m);
    }
}
