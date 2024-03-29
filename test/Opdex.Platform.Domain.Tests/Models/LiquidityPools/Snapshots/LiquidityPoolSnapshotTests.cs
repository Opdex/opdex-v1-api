using FluentAssertions;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots;

public class LiquidityPoolSnapshotTests
{
    [Fact]
    public void CreateLiquidityPoolSnapshot_InvalidLiquidityPoolId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong liquidityPoolId = 0;

        // Act
        void Act() => new LiquidityPoolSnapshot(liquidityPoolId, SnapshotType.Hourly, DateTime.UtcNow);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(liquidityPoolId)} must be greater than 0.");
    }

    [Fact]
    public void CreateLiquidityPoolSnapshot_InvalidSnapshotType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const SnapshotType snapshotType = 0;

        // Act
        void Act() => new LiquidityPoolSnapshot(1, snapshotType, DateTime.UtcNow);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(snapshotType)} must be a valid type.");
    }

    [Fact]
    public void CreateLiquidityPoolSnapshot_Default_Success()
    {
        // Arrange
        const ulong liquidityPoolId = 1;
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
        const ulong liquidityPoolId = 1;
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
        const ulong id = 12345;
        const ulong liquidityPoolId = 124;
        const long transactionCount = 1;
        var reserves = new ReservesSnapshot(new Ohlc<ulong>(100_000_000, 100_000_000, 100_000_000, 100_000_000),
                                            new Ohlc<UInt256>(200000000, 200000000, 200000000, 200000000),
                                            new Ohlc<decimal>(3.00m, 3.00m, 3.00m, 3.00m)); // 1 crs, 2 src,
        var rewards = new RewardsSnapshot(5.00m, 1.00m);
        var staking = new StakingSnapshot(new Ohlc<UInt256>(50000000, 50000000, 50000000, 50000000),
                                          new Ohlc<decimal>(10.00m, 10.00m, 10.00m, 10.00m));
        var volume = new VolumeSnapshot(100, 300, 515.23m);
        var cost = new CostSnapshot(new Ohlc<UInt256>(10, 100, 9, 50), new Ohlc<UInt256>(50, 125, 50, 100));
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount, reserves, rewards, staking, volume, cost,
                                                 snapshotType, startDate, endDate, startDate);

        snapshot.ResetStaleSnapshot(10.00m, .50m, TokenConstants.Cirrus.Sats, startDate.AddDays(1));

        snapshot.Id.Should().Be(0L);
        snapshot.Volume.Should().BeEquivalentTo(new VolumeSnapshot());
        snapshot.Rewards.Should().BeEquivalentTo(new RewardsSnapshot());
        snapshot.Staking.Usd.Should().BeEquivalentTo(new Ohlc<decimal>(staking.Usd.Open, staking.Usd.High, .25m, .25m)); // .5 staking * .5
        snapshot.Staking.Weight.Should().BeEquivalentTo(new Ohlc<UInt256>(staking.Weight.Close, staking.Weight.Close, staking.Weight.Close, staking.Weight.Close));
        snapshot.Cost.CrsPerSrc.Open.Should().Be(cost.CrsPerSrc.Close); // Rolls close to open
        snapshot.Cost.SrcPerCrs.Open.Should().Be(cost.SrcPerCrs.Close); // Rolls close to open
        snapshot.Reserves.Usd.Open.Should().Be(20m);
        snapshot.Reserves.Usd.High.Should().Be(20m);
        snapshot.Reserves.Usd.Low.Should().Be(20m);
        snapshot.Reserves.Usd.Close.Should().Be(20m);
        snapshot.Reserves.Src.Should().Be(reserves.Src);
        snapshot.TransactionCount.Should().Be(0L);
        snapshot.StartDate.Should().Be(startDate.AddDays(1).ToStartOf(snapshotType));
        snapshot.EndDate.Should().Be(startDate.AddDays(1).ToEndOf(snapshotType));
    }

    [Fact]
    public void RefreshSnapshot_Success()
    {
        // Arrange
        const ulong id = 12345;
        const ulong liquidityPoolId = 124;
        const long transactionCount = 1;
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        // Reserves
        const ulong reserveCrs = 100_000_000;
        UInt256 reserveSrc = 200000000;
        const decimal reserveUsd = 3.00m;

        // CrsPerSrc Cost
        UInt256 crsPerSrcOpen = 10;
        UInt256 crsPerSrcHigh = 100;
        UInt256 crsPerSrcLow = 10;
        UInt256 crsPerSrcClose = 90;

        // SrcPerCrsCost
        UInt256 srcPerCrsOpen = 10;
        UInt256 srcPerCrsHigh = 100;
        UInt256 srcPerCrsLow = 10;
        UInt256 srcPerCrsClose = 90;

        // Rewards
        const decimal rewardsProviderUsd = .50m;
        const decimal rewardsMarketUsd = .10m;

        // Staking
        UInt256 stakingWeight = 50000000;
        const decimal stakingUsd = 10.00m;

        // Volume
        const ulong volumeCrs = 100;
        UInt256 volumeSrc = 300;
        const decimal volumeUsd = 515.23m;

        const decimal crsPrice = 10m;
        const decimal stakingTokenPrice = .5m;

        var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                 new ReservesSnapshot(new Ohlc<ulong>(reserveCrs, reserveCrs, reserveCrs, reserveCrs),
                                                                      new Ohlc<UInt256>(reserveSrc, reserveSrc, reserveSrc, reserveSrc),
                                                                      new Ohlc<decimal>(reserveUsd, reserveUsd, reserveUsd, reserveUsd)),                                                     new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                 new StakingSnapshot(new Ohlc<UInt256>(stakingWeight, stakingWeight, stakingWeight, stakingWeight),
                                                                     new Ohlc<decimal>(stakingUsd, stakingUsd, stakingUsd, stakingUsd)),
                                                 new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                 new CostSnapshot(new Ohlc<UInt256>(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                  new Ohlc<UInt256>(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
                                                 snapshotType, startDate, endDate, startDate);

        // Act
        snapshot.RefreshSnapshotFiatAmounts(crsPrice, stakingTokenPrice);

        // Assert
        snapshot.Id.Should().Be(id);
        snapshot.Volume.Crs.Should().Be(volumeCrs);
        snapshot.Volume.Src.Should().Be(volumeSrc);
        snapshot.Volume.Usd.Should().Be(volumeUsd);
        snapshot.Rewards.ProviderUsd.Should().Be(rewardsProviderUsd);
        snapshot.Rewards.MarketUsd.Should().Be(rewardsMarketUsd);
        snapshot.Staking.Weight.Open.Should().Be(stakingWeight);
        snapshot.Staking.Weight.High.Should().Be(stakingWeight);
        snapshot.Staking.Weight.Low.Should().Be(stakingWeight);
        snapshot.Staking.Weight.Close.Should().Be(stakingWeight);
        snapshot.Staking.Usd.Open.Should().Be(stakingUsd);
        snapshot.Staking.Usd.High.Should().Be(stakingUsd);
        snapshot.Staking.Usd.Low.Should().Be(.25m); // .5 staking * .5
        snapshot.Staking.Usd.Close.Should().Be(.25m); // .5 staking * .5
        snapshot.Cost.SrcPerCrs.Open.Should().Be(srcPerCrsOpen);
        snapshot.Cost.SrcPerCrs.High.Should().Be(srcPerCrsHigh);
        snapshot.Cost.SrcPerCrs.Low.Should().Be(srcPerCrsLow);
        snapshot.Cost.SrcPerCrs.Close.Should().Be(srcPerCrsClose);
        snapshot.Cost.CrsPerSrc.Open.Should().Be(crsPerSrcOpen);
        snapshot.Cost.CrsPerSrc.High.Should().Be(crsPerSrcHigh);
        snapshot.Cost.CrsPerSrc.Low.Should().Be(crsPerSrcLow);
        snapshot.Cost.CrsPerSrc.Close.Should().Be(crsPerSrcClose);
        snapshot.Reserves.Crs.Open.Should().Be(reserveCrs);
        snapshot.Reserves.Crs.High.Should().Be(reserveCrs);
        snapshot.Reserves.Crs.Low.Should().Be(reserveCrs);
        snapshot.Reserves.Crs.Close.Should().Be(reserveCrs);
        snapshot.Reserves.Src.Open.Should().Be(reserveSrc);
        snapshot.Reserves.Src.High.Should().Be(reserveSrc);
        snapshot.Reserves.Src.Low.Should().Be(reserveSrc);
        snapshot.Reserves.Src.Close.Should().Be(reserveSrc);
        snapshot.Reserves.Usd.Open.Should().Be(reserveUsd);
        snapshot.Reserves.Usd.High.Should().Be(20m);
        snapshot.Reserves.Usd.Low.Should().Be(reserveUsd);
        snapshot.Reserves.Usd.Close.Should().Be(20m);
        snapshot.TransactionCount.Should().Be(transactionCount);
        snapshot.StartDate.Should().Be(startDate);
        snapshot.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void ProcessSwapLog_Success()
    {
        // Arrange
        const ulong id = 12345;
        const ulong liquidityPoolId = 124;
        const long transactionCount = 1;
        var reserves = new ReservesSnapshot(new Ohlc<ulong>(100_000_000, 100_000_000, 100_000_000, 100_000_000),
                                            new Ohlc<UInt256>(200000000, 200000000, 200000000, 200000000),
                                            new Ohlc<decimal>(3.00m, 3.00m, 3.00m, 3.00m)); // 1 crs, 2 src,
        var rewards = new RewardsSnapshot(.50m, .10m);
        var staking = new StakingSnapshot(new Ohlc<UInt256>(50000000, 50000000, 50000000, 50000000),
                                          new Ohlc<decimal>(10.00m, 10.00m, 10.00m, 10.00m));
        var volume = new VolumeSnapshot(100, 300, 515.23m);
        var cost = new CostSnapshot(new Ohlc<UInt256>(10, 100, 9, 50), new Ohlc<UInt256>(50, 125, 50, 100));
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        dynamic txSwapLog = new ExpandoObject();
        txSwapLog.amountCrsIn = 1_000_000_000ul;
        txSwapLog.amountCrsOut = 0ul;
        txSwapLog.amountSrcIn = "0";
        txSwapLog.amountSrcOut = "2000000000";
        txSwapLog.sender = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK";
        txSwapLog.to = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

        var swapLog = new SwapLog(txSwapLog, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 1);

        var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount, reserves, rewards, staking, volume, cost,
                                                 snapshotType, startDate, endDate, startDate);

        // Act
        snapshot.ProcessSwapLog(swapLog, 10.00m, 1.25m, TokenConstants.Cirrus.Sats, true, 3, true);

        // Assert
        snapshot.Id.Should().Be(id);
        snapshot.Volume.Usd.Should().BeGreaterThan(515.23m);
        snapshot.Volume.Crs.Should().BeGreaterThan(100);
        (snapshot.Volume.Src > 300).Should().Be(true);
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
        const ulong id = 12345;
        const ulong liquidityPoolId = 124;
        const long transactionCount = 1;
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        // Reserves
        const ulong reserveCrs = 100_000_000;
        UInt256 reserveSrc = 200000000;
        const decimal reserveUsd = 3.00m;

        // CrsPerSrc Cost
        UInt256 crsPerSrcOpen = 10;
        UInt256 crsPerSrcHigh = 100;
        UInt256 crsPerSrcLow = 10;
        UInt256 crsPerSrcClose = 90;

        // SrcPerCrsCost
        UInt256 srcPerCrsOpen = 10;
        UInt256 srcPerCrsHigh = 100;
        UInt256 srcPerCrsLow = 10;
        UInt256 srcPerCrsClose = 90;

        // Rewards
        const decimal rewardsProviderUsd = .50m;
        const decimal rewardsMarketUsd = .10m;

        // Staking
        UInt256 stakingWeight = 50000000;
        const decimal stakingUsd = 10.00m;

        // Volume
        const ulong volumeCrs = 100;
        UInt256 volumeSrc = 300;
        const decimal volumeUsd = 515.23m;

        // txLog
        dynamic txReservesLog = new ExpandoObject();
        txReservesLog.reserveCrs = reserveCrs;
        txReservesLog.reserveSrc = reserveSrc.ToString();

        var reservesLog = new ReservesLog(txReservesLog, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 1);

        var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                 new ReservesSnapshot(new Ohlc<ulong>(reserveCrs, reserveCrs, reserveCrs, reserveCrs),
                                                                      new Ohlc<UInt256>(reserveSrc, reserveSrc, reserveSrc, reserveSrc),
                                                                      new Ohlc<decimal>(reserveUsd, reserveUsd, reserveUsd, reserveUsd)),
                                                 new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                 new StakingSnapshot(new Ohlc<UInt256>(stakingWeight, stakingWeight, stakingWeight, stakingWeight),
                                                                     new Ohlc<decimal>(stakingUsd, stakingUsd, stakingUsd, stakingUsd)),
                                                 new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                 new CostSnapshot(new Ohlc<UInt256>(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                  new Ohlc<UInt256>(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
                                                 snapshotType, startDate, endDate, startDate);

        // Act
        snapshot.ProcessReservesLog(reservesLog, 10.00m, TokenConstants.Cirrus.Sats);

        // Assert
        snapshot.Id.Should().Be(id);
        snapshot.Volume.Crs.Should().Be(volumeCrs);
        snapshot.Volume.Src.Should().Be(volumeSrc);
        snapshot.Volume.Usd.Should().Be(volumeUsd);
        snapshot.Rewards.ProviderUsd.Should().Be(rewardsProviderUsd);
        snapshot.Rewards.MarketUsd.Should().Be(rewardsMarketUsd);
        snapshot.Staking.Weight.Should().BeEquivalentTo(new Ohlc<UInt256>(stakingWeight, stakingWeight, stakingWeight, stakingWeight));
        snapshot.Staking.Usd.Should().BeEquivalentTo(new Ohlc<decimal>(stakingUsd, stakingUsd, stakingUsd, stakingUsd));
        snapshot.Cost.SrcPerCrs.Close.Should().NotBe(srcPerCrsOpen).And.NotBe(srcPerCrsClose);
        snapshot.Cost.CrsPerSrc.Close.Should().NotBe(crsPerSrcOpen).And.NotBe(crsPerSrcClose);
        snapshot.Reserves.Crs.Should().BeEquivalentTo(new Ohlc<ulong>(reserveCrs, reserveCrs, reserveCrs, reserveCrs));
        snapshot.Reserves.Src.Should().BeEquivalentTo(new Ohlc<UInt256>(reserveSrc, reserveSrc, reserveSrc, reserveSrc));
        snapshot.Reserves.Usd.Close.Should().BeGreaterThan(reserveUsd);
        snapshot.TransactionCount.Should().Be(transactionCount);
        snapshot.StartDate.Should().Be(startDate);
        snapshot.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void ProcessStakingLog_Success()
    {
        // Arrange
        const ulong id = 12345;
        const ulong liquidityPoolId = 124;
        const long transactionCount = 1;
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        // Reserves
        const ulong reserveCrs = 100_000_000;
        UInt256 reserveSrc = 200000000;
        const decimal reserveUsd = 3.00m;

        // CrsPerSrc Cost
        UInt256 crsPerSrcOpen = 10;
        UInt256 crsPerSrcHigh = 100;
        UInt256 crsPerSrcLow = 10;
        UInt256 crsPerSrcClose = 90;

        // SrcPerCrsCost
        UInt256 srcPerCrsOpen = 10;
        UInt256 srcPerCrsHigh = 100;
        UInt256 srcPerCrsLow = 10;
        UInt256 srcPerCrsClose = 90;

        // Rewards
        const decimal rewardsProviderUsd = .50m;
        const decimal rewardsMarketUsd = .10m;

        // Staking
        UInt256 stakingWeight = 50000000;
        const decimal stakingUsd = 10.00m;

        // Volume
        const ulong volumeCrs = 100;
        UInt256 volumeSrc = 300;
        const decimal volumeUsd = 515.23m;

        // txLog
        UInt256 amount = 2000000000;
        UInt256 totalStaked = 10000000000;
        UInt256 stakerBalance = 100;

        dynamic txStakingLog = new ExpandoObject();
        txStakingLog.staker = "PTotLfm9w7A4KBVq7sJgyP8Hd2MAU8vaRw";
        txStakingLog.amount = amount.ToString();
        txStakingLog.totalStaked = totalStaked.ToString();
        txStakingLog.stakerBalance = stakerBalance.ToString();

        var stakingLog = new StartStakingLog(txStakingLog, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 1);

        var snapshot = new LiquidityPoolSnapshot(id, liquidityPoolId, transactionCount,
                                                 new ReservesSnapshot(new Ohlc<ulong>(reserveCrs, reserveCrs, reserveCrs, reserveCrs),
                                                                      new Ohlc<UInt256>(reserveSrc, reserveSrc, reserveSrc, reserveSrc),
                                                                      new Ohlc<decimal>(reserveUsd, reserveUsd, reserveUsd, reserveUsd)),                                                     new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd),
                                                 new StakingSnapshot(new Ohlc<UInt256>(stakingWeight, stakingWeight, stakingWeight, stakingWeight),
                                                                     new Ohlc<decimal>(stakingUsd, stakingUsd, stakingUsd, stakingUsd)),
                                                 new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd),
                                                 new CostSnapshot(new Ohlc<UInt256>(crsPerSrcOpen, crsPerSrcHigh, crsPerSrcLow, crsPerSrcClose),
                                                                  new Ohlc<UInt256>(srcPerCrsOpen, srcPerCrsHigh, srcPerCrsLow, srcPerCrsClose)),
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
        snapshot.Staking.Weight.Should().BeEquivalentTo(new Ohlc<UInt256>(stakingWeight, totalStaked, stakingWeight, totalStaked));
        snapshot.Staking.Usd.Should().BeEquivalentTo(new Ohlc<decimal>(10m, 100.00m, 10m, 100.00m));
        snapshot.Cost.SrcPerCrs.Open.Should().Be(srcPerCrsOpen);
        snapshot.Cost.SrcPerCrs.High.Should().Be(srcPerCrsHigh);
        snapshot.Cost.SrcPerCrs.Low.Should().Be(srcPerCrsLow);
        snapshot.Cost.SrcPerCrs.Close.Should().Be(srcPerCrsClose);
        snapshot.Cost.CrsPerSrc.Open.Should().Be(crsPerSrcOpen);
        snapshot.Cost.CrsPerSrc.High.Should().Be(crsPerSrcHigh);
        snapshot.Cost.CrsPerSrc.Low.Should().Be(crsPerSrcLow);
        snapshot.Cost.CrsPerSrc.Close.Should().Be(crsPerSrcClose);
        snapshot.Reserves.Crs.Should().BeEquivalentTo(new Ohlc<ulong>(reserveCrs, reserveCrs, reserveCrs, reserveCrs));
        snapshot.Reserves.Src.Should().BeEquivalentTo(new Ohlc<UInt256>(reserveSrc, reserveSrc, reserveSrc, reserveSrc));
        snapshot.Reserves.Usd.Open.Should().Be(reserveUsd);
        snapshot.Reserves.Usd.High.Should().Be(reserveUsd);
        snapshot.Reserves.Usd.Low.Should().Be(reserveUsd);
        snapshot.Reserves.Usd.Close.Should().Be(reserveUsd);
        snapshot.TransactionCount.Should().Be(transactionCount);
        snapshot.StartDate.Should().Be(startDate);
        snapshot.EndDate.Should().Be(endDate);
    }

    [Theory]
    [InlineData(SnapshotType.Hourly)]
    [InlineData(SnapshotType.Minute)]
    public void RewindDailySnapshot_InvalidSnapshotType_ThrowsException(SnapshotType type)
    {
        // Arrange
        var snapshot = new LiquidityPoolSnapshot(1, type, DateTime.UtcNow);
        var hourlySnapshots = new List<LiquidityPoolSnapshot>
        {
            new LiquidityPoolSnapshot(1, type, DateTime.Today)
        };

        // Act
        void Act() => snapshot.RewindDailySnapshot(hourlySnapshots);

        // Assert
        Assert.Throws<Exception>(Act).Message.Should().Contain("Only daily snapshots can be rewound.");
    }

    [Theory]
    [InlineData(SnapshotType.Minute)]
    [InlineData(SnapshotType.Daily)]
    public void RewindDailySnapshot_InvalidSnapshotsTypes_ThrowsException(SnapshotType type)
    {
        // Arrange
        var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
        var hourlySnapshots = new List<LiquidityPoolSnapshot>
        {
            new LiquidityPoolSnapshot(1, type, DateTime.Today)
        };

        // Act
        void Act() => snapshot.RewindDailySnapshot(hourlySnapshots);

        // Assert
        Assert.Throws<Exception>(Act).Message.Should().Contain("Daily snapshots can only rewind using hourly snapshots.");
    }

    [Fact]
    public void RewindDailySnapshot_InvalidSnapshotsLiquidityPoolId_ThrowsException()
    {
        // Arrange
        var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
        var hourlySnapshots = new List<LiquidityPoolSnapshot>
        {
            new LiquidityPoolSnapshot(2, SnapshotType.Hourly, DateTime.Today)
        };

        // Act
        void Act() => snapshot.RewindDailySnapshot(hourlySnapshots);

        // Assert
        Assert.Throws<Exception>(Act).Message.Should().Contain("Daily snapshots can only rewind using hourly snapshots.");
    }

    [Fact]
    public void RewindDailySnapshot_InvalidLSnapshotsDates_ThrowsException()
    {
        // Arrange
        var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
        var hourlySnapshots = new List<LiquidityPoolSnapshot>
        {
            new LiquidityPoolSnapshot(1, SnapshotType.Hourly, DateTime.UtcNow.AddDays(-1).Date)
        };

        // Act
        void Act() => snapshot.RewindDailySnapshot(hourlySnapshots);

        // Assert
        Assert.Throws<Exception>(Act).Message.Should().Contain("Daily snapshots can only rewind using hourly snapshots.");
    }
}