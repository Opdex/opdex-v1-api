using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots;

public class StakingSnapshotTests
{
    [Fact]
    public void CreateStakingSnapshot_Success()
    {
        // Arrange
        var stakingUsd = new Ohlc<decimal>(1.23m, 2.0m, 0.02m, 1.56m);
        var stakingWeight = new Ohlc<UInt256>(123, 123, 123, 123);

        // Act
        var snapshot = new StakingSnapshot(stakingWeight, stakingUsd);

        // Assert
        snapshot.Weight.Should().Be(stakingWeight);
        snapshot.Usd.Should().Be(stakingUsd);
    }

    [Fact]
    public void CreateStakingSnapshot_Default_Success()
    {
        // Arrange
        // Act
        var snapshot = new StakingSnapshot();

        // Assert
        snapshot.Weight.Should().BeEquivalentTo(new Ohlc<UInt256>());
        snapshot.Usd.Should().BeEquivalentTo(new Ohlc<decimal>());
    }

    [Fact]
    public void CreateStakingSnapshot_FromSnapshots_Success()
    {
        // Arrange
        var snapshots = new List<StakingSnapshot>
        {
            new StakingSnapshot(new Ohlc<UInt256>(10, 100, 1, 50), new Ohlc<decimal>(10, 100, 1, 50)),
            new StakingSnapshot(new Ohlc<UInt256>(1, 10, 1, 10), new Ohlc<decimal>(1, 10, 1, 10)),
            new StakingSnapshot(new Ohlc<UInt256>(50, 150, 45, 100), new Ohlc<decimal>(50, 150, 45, 100)),
        };

        var expectedStakingUsd = new Ohlc<decimal>(10, 150, 1, 100);
        var expectedStakingWeight = new Ohlc<UInt256>(10, 150, 1, 100);

        // Act
        var snapshot = new StakingSnapshot(snapshots);

        // Assert
        snapshot.Usd.Should().BeEquivalentTo(expectedStakingUsd);
        snapshot.Weight.Should().BeEquivalentTo(expectedStakingWeight);
    }

    [Fact]
    public void Update_StakingWeight_StartStaking_Success()
    {
        // Arrange
        const decimal stakingTokenUsd = 1m;
        var snapshot = new StakingSnapshot(new Ohlc<UInt256>(10, 90, 1, 50),
                                           new Ohlc<decimal>(.0000001m, .00000009m, .00000001m, .00000005m));

        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = "50";
        txLog.totalStaked = "100";
        txLog.stakerBalance = "50";
        var stakeLog = new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 0);

        var expectedUsd = new Ohlc<decimal>(.0000001m, .000001m, .00000001m, .000001m); // $1 per token * 100 sats closing staked
        var expectedWeight = new Ohlc<UInt256>(10, 100, 1, 100);

        // Act
        snapshot.Update(stakeLog, stakingTokenUsd);

        // Assert
        snapshot.Usd.Should().BeEquivalentTo(expectedUsd);
        snapshot.Weight.Should().BeEquivalentTo(expectedWeight);
    }

    [Fact]
    public void Update_StakingWeight_StopStaking_Success()
    {
        // Arrange
        const decimal stakingTokenUsd = 1m;
        var snapshot = new StakingSnapshot(new Ohlc<UInt256>(10, 120, 80, 100),
                                           new Ohlc<decimal>(.0000001m, .0000012m, .0000008m, .0000001m));

        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = "50";
        txLog.totalStaked = "70";
        txLog.stakerBalance = "0";
        var stakeLog = new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 0);

        var expectedUsd = new Ohlc<decimal>(.0000001m, .0000012m, .0000007m, .0000007m); // $1 per token * 100 sats closing staked
        var expectedWeight = new Ohlc<UInt256>(10, 120, 70, 70);

        // Act
        snapshot.Update(stakeLog, stakingTokenUsd);

        // Assert
        snapshot.Usd.Should().BeEquivalentTo(expectedUsd);
        snapshot.Weight.Should().BeEquivalentTo(expectedWeight);
    }

    [Fact]
    public void Update_StaleUsd_Success()
    {
        // Arrange
        const decimal stakingTokenUsd = 1.5m;

        // priced at $1
        var snapshot = new StakingSnapshot(new Ohlc<UInt256>(10, 120, 80, 100),
                                           new Ohlc<decimal>(.0000001m, .0000012m, .0000008m, .0000001m));

        // new price
        var expectedSnapshot = new StakingSnapshot(new Ohlc<UInt256>(10, 120, 80, 100),
                                                   new Ohlc<decimal>(.0000001m, .0000015m, .0000008m, .0000015m));

        // Act
        snapshot.Update(stakingTokenUsd);

        // Assert
        snapshot.Should().BeEquivalentTo(expectedSnapshot);
    }

    [Fact]
    public void Refresh_Snapshot_Success()
    {
        // Arrange
        const decimal stakingTokenUsd = 1.5m;

        // Priced at $1
        var snapshot = new StakingSnapshot(new Ohlc<UInt256>(10, 120, 80, 100),
                                           new Ohlc<decimal>(.0000001m, .0000012m, .0000008m, .0000001m));

        // new price
        var expectedSnapshot = new StakingSnapshot(new Ohlc<UInt256>(100, 100, 100, 100),
                                                   new Ohlc<decimal>(.0000015m, .0000015m, .0000015m, .0000015m));

        // Act
        snapshot.Refresh(stakingTokenUsd);

        // Assert
        snapshot.Should().BeEquivalentTo(expectedSnapshot);
    }
}