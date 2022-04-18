using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Handlers;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinGeckoApiTests.Handlers;

public class CallCoinGeckoGetStraxHistoricalPriceQueryHandlerTests
{
    private readonly Mock<ICoinsClient> _coinsClientMock;
    private readonly CallCoinGeckoGetStraxHistoricalPriceQueryHandler _handler;

    public CallCoinGeckoGetStraxHistoricalPriceQueryHandlerTests()
    {
        _coinsClientMock = new Mock<ICoinsClient>();
        var coinGeckoClientMock = new Mock<ICoinGeckoClient>();
        coinGeckoClientMock.Setup(callTo => callTo.CoinsClient).Returns(_coinsClientMock.Object);
        _handler = new CallCoinGeckoGetStraxHistoricalPriceQueryHandler(coinGeckoClientMock.Object, new NullLogger<CallCoinGeckoGetStraxHistoricalPriceQueryHandler>());
    }

    [Fact]
    public async Task Handle_ResultIsNull_ReturnZero()
    {
        // Arrange
        _coinsClientMock.Setup(callTo => callTo.GetMarketChartRangeByCoinId(CoinGeckoCoin.Strax, CoinGeckoCoin.Usd,
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MarketChartById)null);

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxHistoricalPriceQuery(DateTime.UtcNow.AddDays(-1)), CancellationToken.None);

        // Assert
        response.Should().Be(Decimal.Zero);
    }

    [Fact]
    public async Task Handle_NoPricesReturned_ReturnZero()
    {
        // Arrange
        _coinsClientMock.Setup(callTo => callTo.GetMarketChartRangeByCoinId(CoinGeckoCoin.Strax, CoinGeckoCoin.Usd,
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MarketChartById
            {
                Prices = Array.Empty<decimal?[]>()
            });

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxHistoricalPriceQuery(DateTime.UtcNow.AddDays(-1)), CancellationToken.None);

        // Assert
        response.Should().Be(Decimal.Zero);
    }

    [Fact]
    public async Task Handle_SinglePriceFoundInRange_ReturnPrice()
    {
        // Arrange
        _coinsClientMock.Setup(callTo => callTo.GetMarketChartRangeByCoinId(CoinGeckoCoin.Strax, CoinGeckoCoin.Usd,
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MarketChartById
            {
                Prices = new[]
                {
                    new decimal?[]{ 435435452243m, 13.25m }
                }
            });

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxHistoricalPriceQuery(DateTime.UtcNow.AddDays(-1)), CancellationToken.None);

        // Assert
        response.Should().Be(13.25m);
    }

    [Fact]
    public async Task Handle_ManyPricesFoundInRange_ReturnPriceAtClosestDateTime()
    {
        // Arrange
        var requestedTime = new DateTime(2022, 01, 11, 15, 00, 00, DateTimeKind.Utc);
        var requestedTimeMs = new DateTimeOffset(requestedTime).ToUnixTimeMilliseconds();
        _coinsClientMock.Setup(callTo => callTo.GetMarketChartRangeByCoinId(CoinGeckoCoin.Strax, CoinGeckoCoin.Usd,
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MarketChartById
            {
                Prices = new[]
                {
                    new decimal?[]{ requestedTimeMs - 50000, 13.56m },
                    new decimal?[]{ requestedTimeMs - 32000, 13.51m },
                    new decimal?[]{ requestedTimeMs - 975, 13.54m }, //  <--- closest
                    new decimal?[]{ requestedTimeMs + 18000, 13.55m },
                }
            });

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxHistoricalPriceQuery(requestedTime), CancellationToken.None);

        // Assert
        response.Should().Be(13.54m);
    }
}
