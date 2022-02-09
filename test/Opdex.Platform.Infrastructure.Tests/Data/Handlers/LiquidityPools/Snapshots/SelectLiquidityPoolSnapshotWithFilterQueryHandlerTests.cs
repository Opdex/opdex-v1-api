using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools.Snapshots;

public class SelectLiquidityPoolSnapshotWithFilterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLiquidityPoolSnapshotWithFilterQueryHandler _handler;

    public SelectLiquidityPoolSnapshotWithFilterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLiquidityPoolSnapshotWithFilterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectLiquidityPoolSnapshotWithFilterQuery(5, DateTime.Now, SnapshotType.Hourly);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectLiquidityPoolSnapshotWithFilter_Success()
    {
        const ulong liquidityPoolId = 8;
        var blockTime = new DateTime(2021, 6, 21, 12, 0, 0);
        const SnapshotType snapshotType = SnapshotType.Daily;

        var expectedEntity = new LiquidityPoolSnapshotEntity
        {
            Id = 123454,
            LiquidityPoolId = liquidityPoolId,
            SnapshotTypeId = (int)snapshotType,
            TransactionCount = 9,
            StartDate = new DateTime(2021, 6, 21),
            EndDate = new DateTime(2021, 6, 21, 23, 59, 59),
            ModifiedDate = DateTime.UtcNow,
            Rewards = new SnapshotRewardsEntity { MarketUsd = 1.23m, ProviderUsd = 9.87m },
            Reserves = new SnapshotReservesEntity
            {
                Crs = new OhlcEntity<ulong> { Open = 123, High = 123, Low = 123, Close = 123},
                Src = new OhlcEntity<UInt256> { Open = 987, High = 987, Low = 987, Close = 987},
                Usd = new OhlcEntity<decimal> {Open = 7.65m, High = 7.65m, Low = 7.65m, Close = 7.65m}
            },
            Volume = new SnapshotVolumeEntity { Crs = 876, Src = 654, Usd = 2.34m },
            Staking = new SnapshotStakingEntity
            {
                Usd = new OhlcEntity<decimal> {Open = 9.12m, High = 9.12m, Low = 9.12m, Close = 9.12m},
                Weight = new OhlcEntity<UInt256> { Open = 648, High = 648, Low = 648, Close = 648},
            },
            Cost = new SnapshotCostEntity
            {
                CrsPerSrc = new OhlcEntity<UInt256> { Open = 1, High = 9, Low = 1, Close = 4 },
                SrcPerCrs = new OhlcEntity<UInt256> { Open = 6, High = 6, Low = 2, Close = 2 }
            }
        };

        var command = new SelectLiquidityPoolSnapshotWithFilterQuery(liquidityPoolId, blockTime, snapshotType);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolSnapshotEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.LiquidityPoolId.Should().Be(liquidityPoolId);
        result.SnapshotType.Should().Be(snapshotType);
        result.TransactionCount.Should().Be(expectedEntity.TransactionCount);
        result.StartDate.Should().Be(expectedEntity.StartDate);
        result.EndDate.Should().Be(expectedEntity.EndDate);
        result.ModifiedDate.Should().Be(expectedEntity.ModifiedDate);

        result.Reserves.Crs.Open.Should().Be(expectedEntity.Reserves.Crs.Open);
        result.Reserves.Crs.High.Should().Be(expectedEntity.Reserves.Crs.High);
        result.Reserves.Crs.Low.Should().Be(expectedEntity.Reserves.Crs.Low);
        result.Reserves.Crs.Close.Should().Be(expectedEntity.Reserves.Crs.Close);

        result.Reserves.Src.Open.Should().Be(expectedEntity.Reserves.Src.Open);
        result.Reserves.Src.High.Should().Be(expectedEntity.Reserves.Src.High);
        result.Reserves.Src.Low.Should().Be(expectedEntity.Reserves.Src.Low);
        result.Reserves.Src.Close.Should().Be(expectedEntity.Reserves.Src.Close);

        result.Reserves.Usd.Open.Should().Be(expectedEntity.Reserves.Usd.Open);
        result.Reserves.Usd.High.Should().Be(expectedEntity.Reserves.Usd.High);
        result.Reserves.Usd.Low.Should().Be(expectedEntity.Reserves.Usd.Low);
        result.Reserves.Usd.Close.Should().Be(expectedEntity.Reserves.Usd.Close);

        result.Rewards.ProviderUsd.Should().Be(expectedEntity.Rewards.ProviderUsd);
        result.Rewards.MarketUsd.Should().Be(expectedEntity.Rewards.MarketUsd);

        result.Volume.Crs.Should().Be(expectedEntity.Volume.Crs);
        result.Volume.Src.Should().Be(expectedEntity.Volume.Src);
        result.Volume.Usd.Should().Be(expectedEntity.Volume.Usd);

        result.Staking.Weight.Open.Should().Be(expectedEntity.Staking.Weight.Open);
        result.Staking.Weight.High.Should().Be(expectedEntity.Staking.Weight.High);
        result.Staking.Weight.Low.Should().Be(expectedEntity.Staking.Weight.Low);
        result.Staking.Weight.Close.Should().Be(expectedEntity.Staking.Weight.Close);

        result.Staking.Usd.Open.Should().Be(expectedEntity.Staking.Usd.Open);
        result.Staking.Usd.High.Should().Be(expectedEntity.Staking.Usd.High);
        result.Staking.Usd.Low.Should().Be(expectedEntity.Staking.Usd.Low);
        result.Staking.Usd.Close.Should().Be(expectedEntity.Staking.Usd.Close);

        result.Cost.CrsPerSrc.Open.Should().Be(expectedEntity.Cost.CrsPerSrc.Open);
        result.Cost.CrsPerSrc.High.Should().Be(expectedEntity.Cost.CrsPerSrc.High);
        result.Cost.CrsPerSrc.Low.Should().Be(expectedEntity.Cost.CrsPerSrc.Low);
        result.Cost.CrsPerSrc.Close.Should().Be(expectedEntity.Cost.CrsPerSrc.Close);

        result.Cost.SrcPerCrs.Open.Should().Be(expectedEntity.Cost.SrcPerCrs.Open);
        result.Cost.SrcPerCrs.High.Should().Be(expectedEntity.Cost.SrcPerCrs.High);
        result.Cost.SrcPerCrs.Low.Should().Be(expectedEntity.Cost.SrcPerCrs.Low);
        result.Cost.SrcPerCrs.Close.Should().Be(expectedEntity.Cost.SrcPerCrs.Close);
    }

    [Fact]
    public async Task SelectLiquidityPoolSnapshotWithFilter_Returns_NewInstance()
    {
        const ulong liquidityPoolId = 1;
        var blockTime = new DateTime(2021, 6, 21, 12, 0, 0);
        const SnapshotType snapshotType = SnapshotType.Daily;

        var command = new SelectLiquidityPoolSnapshotWithFilterQuery(liquidityPoolId, blockTime, snapshotType);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolSnapshotEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<LiquidityPoolSnapshotEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.EndDate.Should().Be(blockTime.ToEndOf(snapshotType));
        result.StartDate.Should().Be(blockTime.ToStartOf(snapshotType));
        result.LiquidityPoolId.Should().Be(liquidityPoolId);
        result.SnapshotType.Should().Be(snapshotType);
        result.TransactionCount.Should().Be(0);

        result.Reserves.Crs.Should().BeEquivalentTo(new OhlcEntity<ulong>());
        result.Reserves.Src.Should().BeEquivalentTo(new OhlcEntity<UInt256>());
        result.Reserves.Usd.Should().BeEquivalentTo(new OhlcEntity<decimal>());

        result.Rewards.ProviderUsd.Should().Be(0.00m);
        result.Rewards.MarketUsd.Should().Be(0.00m);

        result.Volume.Crs.Should().Be(0ul);
        result.Volume.Src.Should().Be(UInt256.Zero);
        result.Volume.Usd.Should().Be(0.00m);

        result.Staking.Weight.Should().BeEquivalentTo(new OhlcEntity<UInt256>());
        result.Staking.Usd.Should().BeEquivalentTo(new OhlcEntity<decimal>());

        result.Cost.CrsPerSrc.Open.Should().Be(UInt256.Zero);
        result.Cost.CrsPerSrc.High.Should().Be(UInt256.Zero);
        result.Cost.CrsPerSrc.Low.Should().Be(UInt256.Zero);
        result.Cost.CrsPerSrc.Close.Should().Be(UInt256.Zero);

        result.Cost.SrcPerCrs.Open.Should().Be(UInt256.Zero);
        result.Cost.SrcPerCrs.High.Should().Be(UInt256.Zero);
        result.Cost.SrcPerCrs.Low.Should().Be(UInt256.Zero);
        result.Cost.SrcPerCrs.Close.Should().Be(UInt256.Zero);
    }
}
