using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools.Snapshots
{
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
        public async Task SelectLiquidityPoolSnapshotWithFilter_Success()
        {
            const long liquidityPoolId = 8;
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
                Reserves = new SnapshotReservesEntity { Crs = 123, Src = "987", Usd = 7.65m },
                Volume = new SnapshotVolumeEntity { Crs = 876, Src = "654", Usd = 2.34m },
                Staking = new SnapshotStakingEntity { Usd = 9.12m, Weight = "648" },
                Cost = new SnapshotCostEntity
                {
                    CrsPerSrc = new OhlcBigIntEntity { Open = "1", High = "9", Low = "1", Close = "4" },
                    SrcPerCrs = new OhlcBigIntEntity { Open = "6", High = "6", Low = "2", Close = "2" }
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

            result.Reserves.Crs.Should().Be(expectedEntity.Reserves.Crs);
            result.Reserves.Src.Should().Be(expectedEntity.Reserves.Src);
            result.Reserves.Usd.Should().Be(expectedEntity.Reserves.Usd);

            result.Rewards.ProviderUsd.Should().Be(expectedEntity.Rewards.ProviderUsd);
            result.Rewards.MarketUsd.Should().Be(expectedEntity.Rewards.MarketUsd);

            result.Volume.Crs.Should().Be(expectedEntity.Volume.Crs);
            result.Volume.Src.Should().Be(expectedEntity.Volume.Src);
            result.Volume.Usd.Should().Be(expectedEntity.Volume.Usd);

            result.Staking.Weight.Should().Be(expectedEntity.Staking.Weight);
            result.Staking.Usd.Should().Be(expectedEntity.Staking.Usd);

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
            const long liquidityPoolId = 1;
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

            result.Reserves.Crs.Should().Be(0ul);
            result.Reserves.Src.Should().Be("0");
            result.Reserves.Usd.Should().Be(0.00m);

            result.Rewards.ProviderUsd.Should().Be(0.00m);
            result.Rewards.MarketUsd.Should().Be(0.00m);

            result.Volume.Crs.Should().Be(0ul);
            result.Volume.Src.Should().Be("0");
            result.Volume.Usd.Should().Be(0.00m);

            result.Staking.Weight.Should().Be("0");
            result.Staking.Usd.Should().Be(0.00m);

            result.Cost.CrsPerSrc.Open.Should().Be("0");
            result.Cost.CrsPerSrc.High.Should().Be("0");
            result.Cost.CrsPerSrc.Low.Should().Be("0");
            result.Cost.CrsPerSrc.Close.Should().Be("0");

            result.Cost.SrcPerCrs.Open.Should().Be("0");
            result.Cost.SrcPerCrs.High.Should().Be("0");
            result.Cost.SrcPerCrs.Low.Should().Be("0");
            result.Cost.SrcPerCrs.Close.Should().Be("0");
        }
    }
}
