using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.EntryHandlers.Addresses;
using Opdex.Platform.Domain.Models.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses
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

            var liqudityPool = new LiquidityPool(5, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10, 15, 20, 25, 30);

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
    }
}
