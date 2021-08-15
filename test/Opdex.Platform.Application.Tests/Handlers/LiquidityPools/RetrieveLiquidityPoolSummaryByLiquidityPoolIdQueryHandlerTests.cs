using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler _handler;

        public RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public void RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("LiquidityPoolId must be greater than 0.");
        }

        [Fact]
        public async Task Handle_MediatorSelectLiquidityPoolSummaryCommand_Send()
        {
            // Arrange
            const long liquidityPoolId = 1;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(liquidityPoolId), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<SelectLiquidityPoolSummaryByLiquidityPoolIdQuery>(query => query.LiquidityPoolId == liquidityPoolId),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorSelectLiquidityPoolSummaryCommand_Return()
        {
            // Arrange
            const long id = 100L;
            const long liquidityPoolId = 1;
            var summary = new LiquidityPoolSummary(id, liquidityPoolId, 2.00m, 3.00m, 4, 5, 7, 8, 9);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectLiquidityPoolSummaryByLiquidityPoolIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(summary);

            // Act
            var response = await _handler.Handle(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(liquidityPoolId), default);

            // Assert
            response.Should().Be(summary);
        }
    }
}
