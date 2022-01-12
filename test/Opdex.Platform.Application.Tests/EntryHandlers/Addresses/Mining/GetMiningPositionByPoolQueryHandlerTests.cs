using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Addresses.Mining;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Mining;

public class GetMiningPositionByPoolQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetMiningPositionByPoolQueryHandler _handler;

    public GetMiningPositionByPoolQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new GetMiningPositionByPoolQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveMiningPoolByAddressQuery_Send()
    {
        // Arrange
        var request = new GetMiningPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByAddressQuery>(query => query.Address == request.MiningPoolAddress && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveAddressMiningByMiningPoolIdAndOwnerQuery_Send()
    {
        // Arrange
        var request = new GetMiningPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
        var cancellationToken = new CancellationTokenSource().Token;

        var miningPool = new MiningPool(5, 5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10"), UInt256.Parse("10"), 10000, 25, 30);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);


        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery>(query => query.MiningPoolId == miningPool.Id
                                                                                                                    && query.Owner == request.Address
                                                                                                                    && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveLiquidityPoolByIdQuery_Send()
    {
        // Arrange
        var request = new GetMiningPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
        var cancellationToken = new CancellationTokenSource().Token;

        var addressMining = new AddressMining(5, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("100000000000"), 50, 500);
        var miningPool = new MiningPool(5, 5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10"), UInt256.Parse("10"), 10000, 25, 30);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressMining);


        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByIdQuery>(query => query.LiquidityPoolId == miningPool.LiquidityPoolId && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveTokenByIdQuery_Send()
    {
        // Arrange
        var request = new GetMiningPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
        var cancellationToken = new CancellationTokenSource().Token;

        var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "ETH-CRS", 10, 15, 20, 25, 30);
        var addressMining = new AddressMining(5, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("100000000000"), 50, 500);
        var miningPool = new MiningPool(5, 5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10"), UInt256.Parse("10"), 10000, 25, 30);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressMining);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == liqudityPool.LpTokenId && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_MapResponse_Return()
    {
        // Arrange
        var request = new GetMiningPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
        var cancellationToken = new CancellationTokenSource().Token;

        var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "GOV-CRS", 10, 15, 20, 25, 30);
        var addressMining = new AddressMining(5, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 100000000000, 50, 500);
        var miningPool = new MiningPool(5, 5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10, 10, 10000, 25, 30);
        var token = new Token(5, "PDrzyNsewpj4KDnDttqcJT5EK7vZXQufNU", "Governance Token", "GOV", 8, 10000000, 10000000000000000000, 10, 20);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressMining);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);


        // Act
        var response = await _handler.Handle(request, cancellationToken);

        // Assert
        response.Address.Should().Be(request.Address);
        response.MiningPool.Should().Be(miningPool.Address);
        response.Amount.Should().Be(FixedDecimal.Parse("1000.00000000"));
        response.MiningToken.Should().Be(token.Address);
    }
}
