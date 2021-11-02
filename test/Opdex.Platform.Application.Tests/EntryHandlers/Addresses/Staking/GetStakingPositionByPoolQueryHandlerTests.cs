using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Addresses.Staking;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Staking
{
    public class GetStakingPositionByPoolQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetStakingPositionByPoolQueryHandler _handler;

        public GetStakingPositionByPoolQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetStakingPositionByPoolQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveLiquidityPoolByAddressQuery_Send()
        {
            // Arrange
            var request = new GetStakingPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(query => query.Address == request.LiquidityPoolAddress && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery_Send()
        {
            // Arrange
            var request = new GetStakingPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
            var cancellationToken = new CancellationTokenSource().Token;

            var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "ETH-CRS", 10, 15, 20, 25, 30);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);


            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery>(query => query.LiquidityPoolId == liqudityPool.Id
                                                                                                                         && query.Owner == request.Address
                                                                                                                         && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveMarketByIdQuery_Send()
        {
            // Arrange
            var request = new GetStakingPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
            var cancellationToken = new CancellationTokenSource().Token;

            var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "ETH-CRS", 10, 15, 20, 25, 30);
            var addressStaking = new AddressStaking(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("50000000000"), 50, 100);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressStaking);


            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByIdQuery>(query => query.MarketId == liqudityPool.MarketId && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveTokenByIdQuery_Send()
        {
            // Arrange
            var request = new GetStakingPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
            var cancellationToken = new CancellationTokenSource().Token;

            var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "ETH-CRS", 10, 15, 20, 25, 30);
            var addressStaking = new AddressStaking(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("50000000000"), 50, 100);
            var market = new Market(5, "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", 5, 25, Address.Empty, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", false, false, false, 3, false, 50, 100);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressStaking);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);


            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == market.StakingTokenId && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MapResponse_Return()
        {
            // Arrange
            var request = new GetStakingPositionByPoolQuery("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX");
            var cancellationToken = new CancellationTokenSource().Token;

            var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "GOV-CRS", 10, 15, 20, 25, 30);
            var addressStaking = new AddressStaking(5, 5, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", UInt256.Parse("50000000000"), 50, 100);
            var market = new Market(5, "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", 5, 25, Address.Empty, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", false, false, false, 3, false, 50, 100);
            var token = new Token(5, "PDrzyNsewpj4KDnDttqcJT5EK7vZXQufNU", false, "Governance Token", "GOV", 8, 10000000, UInt256.Parse("10000000000000000000"), 10, 20);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(liqudityPool);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressStaking);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);


            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.Address.Should().Be(addressStaking.Owner);
            response.LiquidityPool.Should().Be(liqudityPool.Address);
            response.Amount.Should().Be(FixedDecimal.Parse("500.00000000"));
            response.StakingToken.Should().Be(token.Address);
        }
    }
}
