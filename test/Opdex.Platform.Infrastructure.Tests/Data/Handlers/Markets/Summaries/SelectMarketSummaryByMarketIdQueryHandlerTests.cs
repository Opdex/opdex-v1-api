using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Summaries;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Summaries;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Summaries;

public class SelectMarketSummaryByMarketIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectMarketSummaryByMarketIdQueryHandler _handler;

    public SelectMarketSummaryByMarketIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectMarketSummaryByMarketIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectMarketSummaryByMarketId_Success()
    {
        const ulong id = 99ul;

        var expectedEntity = new MarketSummaryEntity
        {
            Id = 1,
            MarketId = 2,
            LiquidityUsd = 3.00m,
            DailyLiquidityUsdChangePercent = 3.50m,
            VolumeUsd = 4.00m,
            StakingWeight = 5,
            DailyStakingWeightChangePercent = 4.5m,
            StakingUsd = 4.5m,
            DailyStakingUsdChangePercent = 6,
            ProviderRewardsDailyUsd = 7,
            MarketRewardsDailyUsd = 7,
            CreatedBlock = 8,
            ModifiedBlock = 9
        };

        var command = new SelectMarketSummaryByMarketIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<MarketSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.MarketId.Should().Be(expectedEntity.MarketId);
        result.LiquidityUsd.Should().Be(expectedEntity.LiquidityUsd);
        result.DailyLiquidityUsdChangePercent.Should().Be(expectedEntity.DailyLiquidityUsdChangePercent);
        result.VolumeUsd.Should().Be(expectedEntity.VolumeUsd);
        result.StakingWeight.Should().Be(expectedEntity.StakingWeight);
        result.DailyStakingWeightChangePercent.Should().Be(expectedEntity.DailyStakingWeightChangePercent);
        result.StakingUsd.Should().Be(expectedEntity.StakingUsd);
        result.DailyStakingUsdChangePercent.Should().Be(expectedEntity.DailyStakingUsdChangePercent);
        result.ProviderRewardsDailyUsd.Should().Be(expectedEntity.ProviderRewardsDailyUsd);
        result.MarketRewardsDailyUsd.Should().Be(expectedEntity.MarketRewardsDailyUsd);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectMarketSummaryByMarketId_Throws_NotFoundException()
    {
        const ulong id = 99ul;

        var command = new SelectMarketSummaryByMarketIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<MarketSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MarketSummaryEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Market summary not found.");
    }

    [Fact]
    public async Task SelectMarketSummaryByMarketId_ReturnsNull()
    {
        const ulong id = 99ul;
        const bool findOrThrow = false;

        var command = new SelectMarketSummaryByMarketIdQuery(id, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<MarketSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MarketSummaryEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
