using System;
using FluentAssertions;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Pools.Snapshots
{
    public class LiquidityPoolSnapshotTests
    {
        [Fact]
        public void CreateLiquidityPoolSnapshot_InvalidLiquidityPoolId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 0;

            // Act
            void Act() => new LiquidityPoolSnapshot(liquidityPoolId, SnapshotType.Hourly, DateTime.UtcNow);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(liquidityPoolId)} must be greater than 0.");
        }

        [Fact]
        public void CreateLiquidityPoolSnapshot_InvalidSnapshotType_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const SnapshotType snapshotType = SnapshotType.Unknown;

            // Act
            void Act() => new LiquidityPoolSnapshot(1, snapshotType, DateTime.UtcNow);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(snapshotType)} must be a valid type.");
        }

        [Fact]
        public void CreateLiquidityPoolSnapshot_Default_Success()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var blockTime = new DateTime(2021, 6, 21, 12, 23, 56);

            // Act
            var snapshot = new LiquidityPoolSnapshot(liquidityPoolId, snapshotType, blockTime);

            // Assert
            snapshot.LiquidityPoolId.Should().Be(liquidityPoolId);
            snapshot.SnapshotType.Should().Be(snapshotType);
            snapshot.StartDate.Should().Be(new DateTime(2021, 6, 21));
            snapshot.EndDate.Should().Be(new DateTime(2021, 6, 21, 23, 59, 59));
            snapshot.Reserves.Should().NotBeNull();
            snapshot.Rewards.Should().NotBeNull();
            snapshot.Staking.Should().NotBeNull();
            snapshot.Volume.Should().NotBeNull();
            snapshot.Cost.Should().NotBeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void LiquidityPoolSnapshot_IncrementTransactionCount_Success(int times)
        {
            // Arrange
            const long liquidityPoolId = 1;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var blockTime = new DateTime(2021, 6, 21, 12, 23, 56);
            var snapshot = new LiquidityPoolSnapshot(liquidityPoolId, snapshotType, blockTime);

            // Act
            for (var i = 0; i < times; i++)
            {
                snapshot.IncrementTransactionCount();
            }

            // Assert
            snapshot.TransactionCount.Should().Be(times);
        }

        [Fact]
        public void ResetStaleSnapshot_Success()
        {
            const long id = 12345;
            const long liquidityPoolId = 124;
            const long transactionCount = 1;
            var reserves = new ReservesSnapshot(100_000_000, "200000000", 3.00m); // 1 crs, 2 src,
            var rewards = new RewardsSnapshot(5.00m, 1.00m);
            var staking = new StakingSnapshot("50000000", 10.00m);
            var volume = new VolumeSnapshot(100, "300", 515.23m);
            var cost = new CostSnapshot(new OhlcBigIntSnapshot("10", "100", "9", "50"), new OhlcBigIntSnapshot("50", "125", "50", "100"));
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount, reserves, rewards, staking, volume, cost,
                                                           snapshotType, startDate, endDate, startDate);

            snapshot.ResetStaleSnapshot(10.00m, 1.25m, .50m, TokenConstants.Cirrus.Sats, startDate.AddDays(1));

            snapshot.Id.Should().Be(0L);
            snapshot.Volume.Should().BeEquivalentTo(new VolumeSnapshot());
            snapshot.Rewards.Should().BeEquivalentTo(new RewardsSnapshot());
            snapshot.Staking.Usd.Should().Be(.25m); // .5 ODX * .5
            snapshot.Staking.Weight.Should().Be(staking.Weight);
            snapshot.Cost.CrsPerSrc.Open.Should().Be(cost.CrsPerSrc.Close); // Rolls close to open
            snapshot.Cost.SrcPerCrs.Open.Should().Be(cost.SrcPerCrs.Close); // Rolls close to open
            snapshot.Reserves.Usd.Should().Be(12.50m); // (1 crs * $10) + (2 src * $1.25)
            snapshot.Reserves.Crs.Should().Be(reserves.Crs);
            snapshot.Reserves.Src.Should().Be(reserves.Src);
            snapshot.TransactionCount.Should().Be(0L);
            snapshot.StartDate.Should().Be(startDate.AddDays(1).ToStartOf(snapshotType));
            snapshot.EndDate.Should().Be(startDate.AddDays(1).ToEndOf(snapshotType));
        }

        [Fact]
        public void RefreshSnapshot_Success()
        {
            // Arrange
            const long id = 12345;
            const long liquidityPoolId = 124;
            const long transactionCount = 1;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            // Reserves
            const long reserveCrs = 100_000_000;
            const string reserveSrc = "200000000";
            const decimal reserveUsd = 3.00m;

            // CrsPerSrc Cost
            const string crsPerSrcOpen = "10";
            const string crsPerSrcHigh = "100";
            const string crsPerSrcLow = "10";
            const string crsPerSrcClose = "90";

            // SrcPerCrsCost
            const string srcPerCrsOpen = "10";
            const string srcPerCrsHigh = "100";
            const string srcPerCrsLow = "10";
            const string srcPerCrsClose = "90";

            // Rewards
            const decimal rewardsProviderUsd = .50m;
            const decimal rewardsMarketUsd = .10m;

            // Staking
            const string stakingWeight = "50000000";
            const decimal stakingUsd = 10.00m;

            // Volume
            const ulong volumeCrs = 100;
            const string volumeSrc = "300";
            const decimal volumeUsd = 515.23m;

            var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                     new ReservesSnapshot(reserveCrs, reserveSrc, reserveUsd),
                                                     new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                     new StakingSnapshot(stakingWeight, stakingUsd),
                                                     new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                     new CostSnapshot(new OhlcBigIntSnapshot(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                      new OhlcBigIntSnapshot(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
                                                     snapshotType, startDate, endDate, startDate);

            // Act
            snapshot.RefreshSnapshot(10.00m, 1.25m, .50m, TokenConstants.Cirrus.Sats);

            // Assert
            snapshot.Id.Should().Be(id);
            snapshot.Volume.Crs.Should().Be(volumeCrs);
            snapshot.Volume.Src.Should().Be(volumeSrc);
            snapshot.Volume.Usd.Should().Be(volumeUsd);
            snapshot.Rewards.ProviderUsd.Should().Be(rewardsProviderUsd);
            snapshot.Rewards.MarketUsd.Should().Be(rewardsMarketUsd);
            snapshot.Staking.Weight.Should().Be(stakingWeight);
            snapshot.Staking.Usd.Should().Be(.25m); // .5 ODX * .5
            snapshot.Cost.SrcPerCrs.Open.Should().Be(srcPerCrsOpen);
            snapshot.Cost.SrcPerCrs.High.Should().Be(srcPerCrsHigh);
            snapshot.Cost.SrcPerCrs.Low.Should().Be(srcPerCrsLow);
            snapshot.Cost.SrcPerCrs.Close.Should().Be(srcPerCrsClose);
            snapshot.Cost.CrsPerSrc.Open.Should().Be(crsPerSrcOpen);
            snapshot.Cost.CrsPerSrc.High.Should().Be(crsPerSrcHigh);
            snapshot.Cost.CrsPerSrc.Low.Should().Be(crsPerSrcLow);
            snapshot.Cost.CrsPerSrc.Close.Should().Be(crsPerSrcClose);
            snapshot.Reserves.Crs.Should().Be(reserveCrs);
            snapshot.Reserves.Src.Should().Be(reserveSrc);
            snapshot.Reserves.Usd.Should().Be(12.50m); // (1 crs * $10) + (2 src * $1.25)
            snapshot.TransactionCount.Should().Be(transactionCount);
            snapshot.StartDate.Should().Be(startDate);
            snapshot.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void ProcessSwapLog_Success()
        {
            // Arrange
            const long id = 12345;
            const long liquidityPoolId = 124;
            const long transactionCount = 1;
            var reserves = new ReservesSnapshot(100_000_000, "200000000", 3.00m); // 1 crs, 2 src,
            var rewards = new RewardsSnapshot(.50m, .10m);
            var staking = new StakingSnapshot("50000000", 10.00m);
            var volume = new VolumeSnapshot(100, "300", 515.23m);
            var cost = new CostSnapshot(new OhlcBigIntSnapshot("10", "100", "9", "50"), new OhlcBigIntSnapshot("50", "125", "50", "100"));
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            dynamic txSwapLog = new ExpandoObject();
            txSwapLog.amountCrsIn = 1_000_000_000ul;
            txSwapLog.amountCrsOut = 0ul;
            txSwapLog.amountSrcIn = "0";
            txSwapLog.amountSrcOut = "2000000000";
            txSwapLog.sender = "Sender";
            txSwapLog.to = "To";

            var swapLog = new SwapLog(txSwapLog, "poolAddress", 1);

            var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount, reserves, rewards, staking, volume, cost,
                                                     snapshotType, startDate, endDate, startDate);

            // Act
            snapshot.ProcessSwapLog(swapLog, 10.00m, 1.25m, TokenConstants.Cirrus.Sats, true, 3, true);

            // Assert
            snapshot.Id.Should().Be(id);
            snapshot.Volume.Usd.Should().BeGreaterThan(515.23m);
            snapshot.Volume.Crs.Should().BeGreaterThan(100);
            snapshot.Volume.Src.ToBigInteger().Should().BeGreaterThan("300".ToBigInteger());
            snapshot.Rewards.ProviderUsd.Should().BeGreaterThan(.50m);
            snapshot.Rewards.MarketUsd.Should().BeGreaterThan(.10m);
            snapshot.Staking.Should().Be(staking);
            snapshot.Cost.Should().BeEquivalentTo(cost);
            snapshot.Reserves.Should().BeEquivalentTo(reserves);
            snapshot.TransactionCount.Should().Be(transactionCount);
            snapshot.StartDate.Should().Be(startDate);
            snapshot.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void ProcessReservesLog_Success()
        {
            // Arrange
            const long id = 12345;
            const long liquidityPoolId = 124;
            const long transactionCount = 1;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            // Reserves
            const long reserveCrs = 100_000_000;
            const string reserveSrc = "200000000";
            const decimal reserveUsd = 3.00m;

            // CrsPerSrc Cost
            const string crsPerSrcOpen = "10";
            const string crsPerSrcHigh = "100";
            const string crsPerSrcLow = "10";
            const string crsPerSrcClose = "90";

            // SrcPerCrsCost
            const string srcPerCrsOpen = "10";
            const string srcPerCrsHigh = "100";
            const string srcPerCrsLow = "10";
            const string srcPerCrsClose = "90";

            // Rewards
            const decimal rewardsProviderUsd = .50m;
            const decimal rewardsMarketUsd = .10m;

            // Staking
            const string stakingWeight = "50000000";
            const decimal stakingUsd = 10.00m;

            // Volume
            const ulong volumeCrs = 100;
            const string volumeSrc = "300";
            const decimal volumeUsd = 515.23m;

            // txLog
            dynamic txReservesLog = new ExpandoObject();
            txReservesLog.reserveCrs = 1_000_000_000ul;
            txReservesLog.reserveSrc = "2000000000";

            var reservesLog = new ReservesLog(txReservesLog, "poolAddress", 1);

            var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                     new ReservesSnapshot(reserveCrs, reserveSrc, reserveUsd),
                                                     new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                     new StakingSnapshot(stakingWeight, stakingUsd),
                                                     new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                     new CostSnapshot(new OhlcBigIntSnapshot(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                      new OhlcBigIntSnapshot(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
                                                     snapshotType, startDate, endDate, startDate);

            // Act
            snapshot.ProcessReservesLog(reservesLog, 10.00m, 1.25m, TokenConstants.Cirrus.Sats);

            // Assert
            snapshot.Id.Should().Be(id);
            snapshot.Volume.Crs.Should().Be(volumeCrs);
            snapshot.Volume.Src.Should().Be(volumeSrc);
            snapshot.Volume.Usd.Should().Be(volumeUsd);
            snapshot.Rewards.ProviderUsd.Should().Be(rewardsProviderUsd);
            snapshot.Rewards.MarketUsd.Should().Be(rewardsMarketUsd);
            snapshot.Staking.Weight.Should().Be(stakingWeight);
            snapshot.Staking.Usd.Should().Be(stakingUsd);
            snapshot.Cost.SrcPerCrs.Close.Should().NotBe(srcPerCrsOpen).And.NotBe(srcPerCrsClose);
            snapshot.Cost.CrsPerSrc.Close.Should().NotBe(crsPerSrcOpen).And.NotBe(crsPerSrcClose);
            snapshot.Reserves.Crs.Should().Be(txReservesLog.reserveCrs);
            snapshot.Reserves.Src.Should().Be(txReservesLog.reserveSrc);
            snapshot.Reserves.Usd.Should().BeGreaterThan(reserveUsd);
            snapshot.TransactionCount.Should().Be(transactionCount);
            snapshot.StartDate.Should().Be(startDate);
            snapshot.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void ProcessStakingLog_Success()
        {
            // Arrange
            const long id = 12345;
            const long liquidityPoolId = 124;
            const long transactionCount = 1;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = new DateTime(2021, 6, 21);
            var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

            // Reserves
            const long reserveCrs = 100_000_000;
            const string reserveSrc = "200000000";
            const decimal reserveUsd = 3.00m;

            // CrsPerSrc Cost
            const string crsPerSrcOpen = "10";
            const string crsPerSrcHigh = "100";
            const string crsPerSrcLow = "10";
            const string crsPerSrcClose = "90";

            // SrcPerCrsCost
            const string srcPerCrsOpen = "10";
            const string srcPerCrsHigh = "100";
            const string srcPerCrsLow = "10";
            const string srcPerCrsClose = "90";

            // Rewards
            const decimal rewardsProviderUsd = .50m;
            const decimal rewardsMarketUsd = .10m;

            // Staking
            const string stakingWeight = "50000000";
            const decimal stakingUsd = 10.00m;

            // Volume
            const ulong volumeCrs = 100;
            const string volumeSrc = "300";
            const decimal volumeUsd = 515.23m;

            // txLog
            dynamic txReservesLog = new ExpandoObject();
            txReservesLog.staker = "StakerAddress";
            txReservesLog.amount = "2000000000";
            txReservesLog.totalStaked = "10000000000";
            txReservesLog.eventType = (byte)1;

            var stakingLog = new StakeLog(txReservesLog, "poolAddress", 1);

            var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                     new ReservesSnapshot(reserveCrs, reserveSrc, reserveUsd),
                                                     new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                     new StakingSnapshot(stakingWeight, stakingUsd),
                                                     new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                     new CostSnapshot(new OhlcBigIntSnapshot(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                      new OhlcBigIntSnapshot(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
                                                     snapshotType, startDate, endDate, startDate);

            // Act
            snapshot.ProcessStakingLog(stakingLog, 1.00m);

            // Assert
            snapshot.Id.Should().Be(id);
            snapshot.Volume.Crs.Should().Be(volumeCrs);
            snapshot.Volume.Src.Should().Be(volumeSrc);
            snapshot.Volume.Usd.Should().Be(volumeUsd);
            snapshot.Rewards.ProviderUsd.Should().Be(rewardsProviderUsd);
            snapshot.Rewards.MarketUsd.Should().Be(rewardsMarketUsd);
            snapshot.Staking.Weight.Should().Be(txReservesLog.totalStaked);
            snapshot.Staking.Usd.Should().Be(100.00m);
            snapshot.Cost.SrcPerCrs.Open.Should().Be(srcPerCrsOpen);
            snapshot.Cost.SrcPerCrs.High.Should().Be(srcPerCrsHigh);
            snapshot.Cost.SrcPerCrs.Low.Should().Be(srcPerCrsLow);
            snapshot.Cost.SrcPerCrs.Close.Should().Be(srcPerCrsClose);
            snapshot.Cost.CrsPerSrc.Open.Should().Be(crsPerSrcOpen);
            snapshot.Cost.CrsPerSrc.High.Should().Be(crsPerSrcHigh);
            snapshot.Cost.CrsPerSrc.Low.Should().Be(crsPerSrcLow);
            snapshot.Cost.CrsPerSrc.Close.Should().Be(crsPerSrcClose);
            snapshot.Reserves.Crs.Should().Be(reserveCrs);
            snapshot.Reserves.Src.Should().Be(reserveSrc);
            snapshot.Reserves.Usd.Should().Be(reserveUsd);
            snapshot.TransactionCount.Should().Be(transactionCount);
            snapshot.StartDate.Should().Be(startDate);
            snapshot.EndDate.Should().Be(endDate);
        }
    }
}
