using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Summaries;

public class ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler _handler;

    public ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandlerTests()
    {
        var logger = new NullLogger<ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommandHandler(_dbContext.Object, logger);
    }

    [Fact]
    public void ExecuteUpdateMarketSummaryLiquidityPoolCountCommand_MarketIdZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => _ = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(0, 5);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).ParamName.Should().Be("marketId");
    }

    [Fact]
    public void ExecuteUpdateMarketSummaryLiquidityPoolCountCommand_BlockHeightZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => _ = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(5, 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).ParamName.Should().Be("blockHeight");
    }

    [Fact]
    public async Task ExecuteUpdateMarketSummaryLiquidityPoolCountCommand_Executes_StoredProcedure()
    {
        // Arrange
        var command = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(5, 10);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Parameters != null &&
                                                                                         q.Type == CommandType.StoredProcedure &&
                                                                                         q.Sql == "UpdateMarketSummaryLiquidityPoolCount")), Times.Once);
    }

    [Fact]
    public async Task ExecuteUpdateMarketSummaryLiquidityPoolCountCommand_Success()
    {
        // Arrange
        var command = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(5 , 10);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteUpdateMarketSummaryLiquidityPoolCountCommand_Fail()
    {
        // Arrange
        var command = new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(5, 10);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
