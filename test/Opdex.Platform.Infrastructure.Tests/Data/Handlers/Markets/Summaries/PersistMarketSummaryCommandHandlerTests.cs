using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Summaries;

public class PersistMarketSummaryCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistMarketSummaryCommandHandler _handler;

    public PersistMarketSummaryCommandHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        var logger = new NullLogger<PersistMarketSummaryCommandHandler>();

        _dbContext = new Mock<IDbContext>();
        _handler = new PersistMarketSummaryCommandHandler(_dbContext.Object, mapper, logger);
    }

    [Fact]
    public async Task Insert_MarketSummary_SendsSqlCommand()
    {
        var model = new MarketSummary(1, 8);
        var command = new PersistMarketSummaryCommand(model);

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        } catch { }

        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<ulong>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("INSERT INTO market_summary")
                                                        && q.Sql.Contains("SELECT LAST_INSERT_ID();"))));
    }

    [Fact]
    public async Task Update_MarketSummary_SendsSqlCommand()
    {
        const ulong expectedId = 10ul;
        var model = new MarketSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9, 50, 10, 11);
        var command = new PersistMarketSummaryCommand(model);

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        } catch { }

        _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<ulong>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("UPDATE market_summary")
                                                        && q.Sql.Contains("WHERE Id = @Id;"))));
    }

    [Fact]
    public async Task Insert_MarketSummary_Returns()
    {
        const ulong expectedId = 10ul;
        var model = new MarketSummary(1, 8);
        var command = new PersistMarketSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_MarketSummary_Returns()
    {
        const ulong expectedId = 10ul;
        var model = new MarketSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9, 50, 10, 11);
        var command = new PersistMarketSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsMarketSummary_Fail()
    {
        const ulong expectedId = 0;
        var model = new MarketSummary(expectedId, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 7, 8, 9, 50, 10, 11);
        var command = new PersistMarketSummaryCommand(model);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Throws(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
