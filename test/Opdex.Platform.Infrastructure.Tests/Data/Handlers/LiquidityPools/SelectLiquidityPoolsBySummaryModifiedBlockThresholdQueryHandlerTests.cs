using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools;

public class SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler _handler;

    public SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLiquidityPoolsBySummaryModifiedBlockThresholdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public void SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery_InvalidBlockThreshold_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block threshold must be greater than zero.");
    }

    [Fact]
    public async Task SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery_Success()
    {
        const ulong blockThreshold = 50;

        var expectedEntity = new LiquidityPoolEntity
        {
            Id = 123454,
            SrcTokenId = 1235,
            LpTokenId = 8765,
            MarketId = 1,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            CreatedBlock = 1,
            ModifiedBlock = 1
        };

        var command = new SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery(blockThreshold);

        _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(new [] { expectedEntity });

        var results = await _handler.Handle(command, CancellationToken.None);

        var resultsList = results.ToList();
        resultsList[0].Id.Should().Be(expectedEntity.Id);
        resultsList[0].SrcTokenId.Should().Be(expectedEntity.SrcTokenId);
        resultsList[0].LpTokenId.Should().Be(expectedEntity.LpTokenId);
        resultsList[0].MarketId.Should().Be(expectedEntity.MarketId);
        resultsList[0].Address.Should().Be(expectedEntity.Address);
        resultsList[0].CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        resultsList[0].ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }
}
