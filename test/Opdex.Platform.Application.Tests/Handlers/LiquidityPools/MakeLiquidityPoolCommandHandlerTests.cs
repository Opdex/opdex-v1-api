using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Handlers.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.LiquidityPools;

public class MakeLiquidityPoolCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MakeLiquidityPoolCommandHandler _handler;

    public MakeLiquidityPoolCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new MakeLiquidityPoolCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public void MakeLiquidityPoolCommand_InvalidSummary_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        void Act() => _ = new MakeLiquidityPoolCommand(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool must be set.");
    }

    [Fact]
    public async Task MakeLiquidityPoolCommand_Sends_PersistLiquidityPoolCommand()
    {
        // Arrange
        var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "ETH-CRS", 5, 15, 25, 500, 505);

        // Act
        await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<PersistLiquidityPoolCommand>(c => c.Pool == liquidityPool), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task MakeLiquidityPoolCommand_Sends_ExecuteUpdateMarketSummaryLiquidityPoolCountCommand()
    {
        // Arrange
        var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "ETH-CRS", 5, 15, 25, 500, 505);

        // Act
        await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<ExecuteUpdateMarketSummaryLiquidityPoolCountCommand>(
            c => c.MarketId == liquidityPool.MarketId && c.BlockHeight == liquidityPool.CreatedBlock), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task MakeLiquidityPoolCommand_Returns_PersistLiquidityPoolCommandResult()
    {
        // Arrange
        const ulong id = 5;

        var liquidityPool = new LiquidityPool(5, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "ETH-CRS", 5, 15, 25, 500, 505);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistLiquidityPoolCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(id);

        // Act
        var result = await _handler.Handle(new MakeLiquidityPoolCommand(liquidityPool), CancellationToken.None);

        // Assert
        result.Should().Be(id);
    }
}
