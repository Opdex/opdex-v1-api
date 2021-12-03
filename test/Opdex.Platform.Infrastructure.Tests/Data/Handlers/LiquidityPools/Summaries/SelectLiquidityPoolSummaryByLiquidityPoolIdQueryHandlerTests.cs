using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Summaries;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools.Summaries;

public class SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler _handler;

    public SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLiquidityPoolSummaryByLiquidityPoolIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectLiquidityPoolSummaryByLiquidityPoolId_Success()
    {
        const ulong id = 99ul;

        var expectedEntity = new LiquidityPoolSummaryEntity
        {
            Id = 1,
            LiquidityPoolId = 2,
            LiquidityUsd = 3.00m,
            DailyLiquidityUsdChangePercent = 3.50m,
            VolumeUsd = 4.00m,
            StakingWeight = 5,
            DailyStakingWeightChangePercent = 4.5m,
            LockedCrs = 6,
            LockedSrc = 7,
            CreatedBlock = 8,
            ModifiedBlock = 9
        };

        var command = new SelectLiquidityPoolSummaryByLiquidityPoolIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.LiquidityPoolId.Should().Be(expectedEntity.LiquidityPoolId);
        result.LiquidityUsd.Should().Be(expectedEntity.LiquidityUsd);
        result.DailyLiquidityUsdChangePercent.Should().Be(expectedEntity.DailyLiquidityUsdChangePercent);
        result.VolumeUsd.Should().Be(expectedEntity.VolumeUsd);
        result.StakingWeight.Should().Be(expectedEntity.StakingWeight);
        result.DailyStakingWeightChangePercent.Should().Be(expectedEntity.DailyStakingWeightChangePercent);
        result.LockedCrs.Should().Be(expectedEntity.LockedCrs);
        result.LockedSrc.Should().Be(expectedEntity.LockedSrc);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectLiquidityPoolSummaryByLiquidityPoolId_Throws_NotFoundException()
    {
        const ulong id = 99ul;

        var command = new SelectLiquidityPoolSummaryByLiquidityPoolIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<LiquidityPoolSummaryEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(LiquidityPoolSummary)} not found.");
    }

    [Fact]
    public async Task SelectLiquidityPoolSummaryByLiquidityPoolId_ReturnsNull()
    {
        const ulong id = 99ul;
        const bool findOrThrow = false;

        var command = new SelectLiquidityPoolSummaryByLiquidityPoolIdQuery(id, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolSummaryEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<LiquidityPoolSummaryEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}