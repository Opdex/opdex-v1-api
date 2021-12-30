using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets;

public class MarketSummaryTests
{
    [Fact]
    public void CreateNew_MarketSummary_InvalidMarketId_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        const ulong marketId = 0;
        const ulong createdBlock = 7;

        // Act
        void Act() => new MarketSummary(marketId, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("MarketId must be greater than zero.");
    }

    [Fact]
    public void CreateExistingFromEntity_MarketSummary_Success()
    {
        // Arrange
        const ulong id = 10;
        const ulong marketId = 1;
        const decimal dailyLiquidityChange = 1.5m;
        const decimal liquidity = 2.00m;
        const decimal volume = 3.00m;
        const ulong stakingWeight = 4;
        const decimal dailyStakingChange = 2.5m;
        const decimal stakingUsd = 3.23m;
        const decimal dailyStakingUsdChange = 0.12m;
        const decimal providerRewardsDailyUsd = 1.12m;
        const decimal marketRewardsDailyUsd = 0.22m;
        const ulong createdBlock = 7;
        const ulong modifiedBlock = 8;

        // Act
        var result = new MarketSummary(id, marketId, liquidity, dailyLiquidityChange, volume, stakingWeight, dailyStakingChange, stakingUsd,
                                       dailyStakingUsdChange, providerRewardsDailyUsd, marketRewardsDailyUsd, createdBlock, modifiedBlock);

        // Assert
        result.Id.Should().Be(id);
        result.MarketId.Should().Be(marketId);
        result.LiquidityUsd.Should().Be(liquidity);
        result.VolumeUsd.Should().Be(volume);
        result.StakingWeight.Should().Be(stakingWeight);
        result.DailyStakingWeightChangePercent.Should().Be(dailyStakingChange);
        result.StakingUsd.Should().Be(stakingUsd);
        result.DailyStakingUsdChangePercent.Should().Be(dailyStakingUsdChange);
        result.ProviderRewardsDailyUsd.Should().Be(providerRewardsDailyUsd);
        result.MarketRewardsDailyUsd.Should().Be(marketRewardsDailyUsd);
        result.CreatedBlock.Should().Be(createdBlock);
        result.ModifiedBlock.Should().Be(modifiedBlock);
    }

    [Fact]
    public void Updates_MarketSummary_Success()
    {
        // Arrange
        const ulong marketId = 1;
        const ulong createdBlock = 7;
        const ulong modifiedBlock = 8;

        var summary = new MarketSummary(marketId, createdBlock);
        var snapshot = GetMarketSnapshot(marketId);

        // Act
        summary.Update(snapshot, modifiedBlock);

        // Assert
        summary.LiquidityUsd.Should().Be(snapshot.LiquidityUsd.Close);
        summary.VolumeUsd.Should().Be(snapshot.VolumeUsd);
        summary.StakingWeight.Should().Be((ulong)snapshot.Staking.Weight.Close);
        summary.StakingUsd.Should().Be(snapshot.Staking.Usd.Close);
        summary.ProviderRewardsDailyUsd.Should().Be(snapshot.Rewards.ProviderUsd);
        summary.MarketRewardsDailyUsd.Should().Be(snapshot.Rewards.MarketUsd);
    }

    private static MarketSnapshot GetMarketSnapshot(ulong marketId)
    {
        const ulong id = 12345;
        var liquidityUsd = new Ohlc<decimal>(3m, 3m, 3m, 3m);
        const decimal volumeUsd = 5m;
        var staking = new StakingSnapshot(new Ohlc<UInt256>(50000000,50000000,50000000,50000000), new Ohlc<decimal>(10.00m, 10.00m, 10.00m, 10.00m));
        var rewards = new RewardsSnapshot(5.00m, 1.00m);
        const SnapshotType snapshotType = SnapshotType.Daily;
        var startDate = new DateTime(2021, 6, 21);
        var endDate = new DateTime(2021, 6, 21, 23, 59, 59);

        return new MarketSnapshot(id, marketId, liquidityUsd, volumeUsd, staking, rewards, snapshotType, startDate, endDate);
    }
}
