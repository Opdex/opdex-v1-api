using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools.Summaries;

public class PersistLiquidityPoolSummaryCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistLiquidityPoolSummaryCommandHandler _handler;

    public PersistLiquidityPoolSummaryCommandHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        var logger = new NullLogger<PersistLiquidityPoolSummaryCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new PersistLiquidityPoolSummaryCommandHandler(_dbContext.Object, mapper, logger);
    }

    [Fact]
    public async Task Insert_LiquidityPoolSummary_SendsSqlCommand()
    {
        var model = new LiquidityPoolSummary(1, 8);
        var command = new PersistLiquidityPoolSummaryCommand(model);

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        } catch { }

        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<ulong>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("INSERT INTO pool_liquidity_summary")
                                                        && q.Sql.Contains("SELECT LAST_INSERT_ID();"))));
    }

    [Fact]
    public async Task Update_LiquidityPoolSummary_SendsSqlCommand()
    {
        const ulong expectedId = 10ul;
        var model = new LiquidityPoolSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9);
        var command = new PersistLiquidityPoolSummaryCommand(model);

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        } catch { }

        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<ulong>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("UPDATE pool_liquidity_summary")
                                                        && q.Sql.Contains("WHERE Id = @Id;"))));
    }

    [Fact]
    public async Task Insert_LiquidityPoolSummary_Returns()
    {
        const ulong expectedId = 10ul;
        var model = new LiquidityPoolSummary(1, 8);
        var command = new PersistLiquidityPoolSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_LiquidityPoolSummary_Returns()
    {
        const ulong expectedId = 10ul;
        var model = new LiquidityPoolSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9);
        var command = new PersistLiquidityPoolSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsLiquidityPoolSummary_Fail()
    {
        const ulong expectedId = 0;
        var model = new LiquidityPoolSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9);
        var command = new PersistLiquidityPoolSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Throws(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}