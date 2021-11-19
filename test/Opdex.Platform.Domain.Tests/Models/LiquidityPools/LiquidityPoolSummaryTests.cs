using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools
{
    public class LiquidityPoolSummaryTests
    {
        [Fact]
        public void CreateNew_LiquidityPoolSummary_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong liquidityPoolId = 0;
            const ulong createdBlock = 7;

            // Act
            void Act() => new LiquidityPoolSummary(liquidityPoolId, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("LiquidityPoolId must be greater than 0.");
        }

        [Fact]
        public void CreateExistingFromEntity_LiquidityPoolSummary_Success()
        {
            // Arrange
            const ulong id = 10;
            const ulong liquidityPoolId = 1;
            const decimal liquidity = 2.00m;
            const decimal volume = 3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;
            const ulong modifiedBlock = 8;

            // Act
            var result = new LiquidityPoolSummary(id, liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock, modifiedBlock);

            // Assert
            result.Id.Should().Be(id);
            result.LiquidityPoolId.Should().Be(liquidityPoolId);
            result.LiquidityUsd.Should().Be(liquidity);
            result.VolumeUsd.Should().Be(volume);
            result.StakingWeight.Should().Be(stakingWeight);
            result.LockedCrs.Should().Be(lockedCrs);
            result.LockedSrc.Should().Be(lockedSrc);
            result.CreatedBlock.Should().Be(createdBlock);
            result.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void Updates_LiquidityPoolSummary_Success()
        {
            // Arrange
            const ulong liquidityPoolId = 1;
            const ulong createdBlock = 7;
            const ulong modifiedBlock = 8;

            var summary = new LiquidityPoolSummary(liquidityPoolId, createdBlock);
            var snapshot = GetLiquidityPoolSnapshot(liquidityPoolId);

            // Act
            summary.Update(snapshot, modifiedBlock);

            // Assert
            summary.LiquidityUsd.Should().Be(snapshot.Reserves.Usd.Close);
            summary.VolumeUsd.Should().Be(snapshot.Volume.Usd);
            summary.StakingWeight.Should().Be((ulong)snapshot.Staking.Weight.Close);
            summary.LockedCrs.Should().Be(snapshot.Reserves.Crs.Close);
            summary.LockedSrc.Should().Be(snapshot.Reserves.Src.Close);
        }

        private static LiquidityPoolSnapshot GetLiquidityPoolSnapshot(ulong liquidityPoolId)
        {
            const ulong id = 12345;
            const long transactionCount = 1;
            var reserves = new ReservesSnapshot(new Ohlc<ulong>(100_000_000, 100_000_000, 100_000_000, 100_000_000),
                                                new Ohlc<UInt256>(200000000, 200000000, 200000000, 200000000),
                                                new Ohlc<decimal>(3m, 3m, 3m, 3m)); // 1 crs, 2 src,
            var staking = new StakingSnapshot(new Ohlc<UInt256>(50000000,50000000,50000000,50000000),
                                              new Ohlc<decimal>(10.00m, 10.00m, 10.00m, 10.00m));
            var rewards = new RewardsSnapshot(5.00m, 1.00m);
            var volume = new VolumeSnapshot(100, 300, 515.23m);
            var cost = new CostSnapshot(new Ohlc<UInt256>(10, 100, 9, 50), new Ohlc<UInt256>(50, 125, 50, 100));
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            return new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount, reserves, rewards, staking, volume, cost, snapshotType,
                                             startDate, endDate, startDate);
        }
    }
}
