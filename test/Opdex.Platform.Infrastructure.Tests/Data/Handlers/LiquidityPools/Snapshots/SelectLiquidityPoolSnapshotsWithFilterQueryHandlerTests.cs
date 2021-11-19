using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools.Snapshots
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
            const ulong liquidityPoolId = 8;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);
            const SnapshotType snapshotType = SnapshotType.Daily;
            var cursor = new SnapshotCursor(Interval.OneDay, startDate, endDate, SortDirectionType.ASC, 10, PagingDirection.Forward, default);

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
                    Crs = new OhlcEntity<ulong>{ Open = 123, High = 123, Low = 123, Close = 123 },
                    Src = new OhlcEntity<UInt256>{ Open = 987, High = 987, Low = 987, Close = 987 },
                    Usd = new OhlcEntity<decimal>{ Open = 1, High = 9, Low = 1, Close = 4 }
                },
                Volume = new SnapshotVolumeEntity { Crs = 876, Src = 654, Usd = 2.34m },
                Staking = new SnapshotStakingEntity
                {
                    Usd = new OhlcEntity<decimal>{ Open = 9.12m, High = 9.12m, Low = 9.12m, Close = 9.12m },
                    Weight = new OhlcEntity<UInt256> { Open = 648, High = 648, Low = 648, Close = 648 }
                },
                Cost = new SnapshotCostEntity
                {
                    CrsPerSrc = new OhlcEntity<UInt256> { Open = 1, High = 9, Low = 1, Close = 4 },
                    SrcPerCrs = new OhlcEntity<UInt256> { Open = 6, High = 6, Low = 2, Close = 2 }
                }
            };

            var entities = new List<LiquidityPoolSnapshotEntity> { expectedEntity };

            var command = new SelectLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolId, cursor);

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
        }

        [Fact]
        public async Task SelectLiquidityPoolSnapshotsWithFilter_Returns_EmptyList()
        {
            const ulong liquidityPoolId = 1;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);
            var cursor = new SnapshotCursor(Interval.OneDay, startDate, endDate, SortDirectionType.ASC, 10, PagingDirection.Forward, default);
            var command = new SelectLiquidityPoolSnapshotsWithFilterQuery(liquidityPoolId, cursor);

            _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<IEnumerable<LiquidityPoolSnapshotEntity>>(null));

            var results = await _handler.Handle(command, CancellationToken.None);

            results.Count().Should().Be(0);
        }
    }
}
