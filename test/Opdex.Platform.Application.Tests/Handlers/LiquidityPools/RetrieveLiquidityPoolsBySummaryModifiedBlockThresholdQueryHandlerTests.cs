using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools;

public class RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler _handler;

    public RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public void RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery_InvalidBlockThreshold_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block threshold must be greater than zero.");
    }

    [Fact]
    public async Task Handle_SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery_Send()
    {
        // Arrange
        const ulong blockThreshold = 50;
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(new RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(blockThreshold), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
                                 It.Is<SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery>(query => query.BlockThreshold == blockThreshold),
                                 cancellationToken
                             ), Times.Once);
    }
}
