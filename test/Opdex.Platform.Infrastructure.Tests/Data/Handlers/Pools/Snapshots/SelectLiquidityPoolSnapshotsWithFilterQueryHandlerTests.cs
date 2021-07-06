using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools.Snapshots
{
    public class SelectLiquidityPoolSnapshotsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectLiquidityPoolSnapshotsWithFilterQueryHandler _handler;

        public SelectLiquidityPoolSnapshotsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectLiquidityPoolSnapshotsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectLiquidityPoolSnapshotsWithFilter_Success()
        {
            const long liquidityPoolId = 8;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);
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

            var entities = new List<LiquidityPoolSnapshotEntity> {expectedEntity};

            var command = new SelectLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolId, startDate, endDate, snapshotType);

            _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(entities.AsEnumerable()));

            var results = await _handler.Handle(command, CancellationToken.None);

            var resultsList = results.ToList();

            for (var i = 0; i < resultsList.Count; i++)
            {
                var result = resultsList[i];

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
        }

        [Fact]
        public async Task SelectLiquidityPoolSnapshotsWithFilter_Returns_EmptyList()
        {
            const long liquidityPoolId = 1;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);
            const SnapshotType snapshotType = SnapshotType.Daily;

            var command = new SelectLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolId, startDate, endDate, snapshotType);

            _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<IEnumerable<LiquidityPoolSnapshotEntity>>(null));

            var results = await _handler.Handle(command, CancellationToken.None);

            results.Count().Should().Be(0);
        }
    }
}
