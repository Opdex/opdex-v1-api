using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools;

public class MakeLiquidityPoolSummaryCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MakeLiquidityPoolSummaryCommandHandler _handler;

    public MakeLiquidityPoolSummaryCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new MakeLiquidityPoolSummaryCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public void MakeLiquidityPoolSummaryCommand_InvalidSummary_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        void Act() => new MakeLiquidityPoolSummaryCommand(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool summary must be set.");
    }

    [Fact]
    public async Task Handle_MediatorPersistLiquidityPoolSummaryCommand_Send()
    {
        // Arrange
        var summary = new LiquidityPoolSummary(1, 8);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(new MakeLiquidityPoolSummaryCommand(summary), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
                                 It.Is<PersistLiquidityPoolSummaryCommand>(command => command.Summary == summary),
                                 cancellationToken
                             ), Times.Once);
    }

    [Fact]
    public async Task Handle_MediatorPersistLiquidityPoolSummaryCommand_Return()
    {
        // Arrange
        var id = 5ul;
        var summary = new LiquidityPoolSummary(1, 8);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistLiquidityPoolSummaryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(id);

        // Act
        var response = await _handler.Handle(new MakeLiquidityPoolSummaryCommand(summary), default);

        // Assert
        response.Should().Be(id);
    }
}