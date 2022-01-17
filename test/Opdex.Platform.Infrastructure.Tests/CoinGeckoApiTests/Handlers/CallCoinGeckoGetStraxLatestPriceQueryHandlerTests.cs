using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;
using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinGeckoApiTests.Handlers;

public class CallCoinGeckoGetStraxLatestPriceQueryHandlerTests
{
    private readonly Mock<ICoinsClient> _coinsClientMock;
    private readonly CallCoinGeckoGetStraxLatestPriceQueryHandler _handler;

    public CallCoinGeckoGetStraxLatestPriceQueryHandlerTests()
    {
        _coinsClientMock = new Mock<ICoinsClient>();
        var coinGeckoClientMock = new Mock<ICoinGeckoClient>();
        coinGeckoClientMock.Setup(callTo => callTo.CoinsClient).Returns(_coinsClientMock.Object);
        _handler = new CallCoinGeckoGetStraxLatestPriceQueryHandler(coinGeckoClientMock.Object, new NullLogger<CallCoinGeckoGetStraxLatestPriceQueryHandler>());
    }

    [Fact]
    public async Task Handle_NullResult_ReturnZero()
    {
        // Arrange
        _coinsClientMock.Setup(callTo => callTo.GetAllCoinDataWithId(CoinGeckoTokens.STRAX, It.IsAny<string>(),
                It.IsAny<bool>(), true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((CoinFullDataById)null);

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxLatestPriceQuery(), CancellationToken.None);

        // Assert
        response.Should().Be(Decimal.Zero);
    }

    [Fact]
    public async Task Handle_NullValue_ReturnZero()
    {
        // Arrange
        _coinsClientMock.Setup(callTo => callTo.GetAllCoinDataWithId(CoinGeckoTokens.STRAX, It.IsAny<string>(),
                It.IsAny<bool>(), true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new CoinFullDataById()
            {
                MarketData = new CoinByIdMarketData()
                {
                    CurrentPrice = new Dictionary<string, decimal?>() { { CoinGeckoTokens.USD, null } }
                }
            });

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxLatestPriceQuery(), CancellationToken.None);

        // Assert
        response.Should().Be(Decimal.Zero);
    }

    [Fact]
    public async Task Handle_NonNullResult_ReturnExpected()
    {
        // Arrange
        decimal expected = 12.34M;
        _coinsClientMock.Setup(callTo => callTo.GetAllCoinDataWithId(CoinGeckoTokens.STRAX, It.IsAny<string>(),
                It.IsAny<bool>(), true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new CoinFullDataById()
            {
                MarketData = new CoinByIdMarketData()
                {
                    CurrentPrice = new Dictionary<string, decimal?>() { { CoinGeckoTokens.USD, expected } }
                }
            });

        // Act
        var response = await _handler.Handle(new CallCoinGeckoGetStraxLatestPriceQuery(), CancellationToken.None);

        // Assert
        response.Should().Be(expected);
    }
}
