using System;
using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets
{
    public class MarketSnapshotTests
    {
        [Fact]
        public void CreateNewMarketSnapshot_Success()
        {
            const ulong marketId = 2;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var date = new DateTime(2021, 6, 22, 12, 23, 32);

            var marketSnapshot = new MarketSnapshot(marketId, snapshotType, date);

            marketSnapshot.MarketId.Should().Be(marketId);
            marketSnapshot.Liquidity.Should().Be(0.00m);
            marketSnapshot.Volume.Should().Be(0.00m);
            marketSnapshot.Staking.Weight.Should().BeEquivalentTo(new Ohlc<UInt256>());
            marketSnapshot.Staking.Usd.Should().BeEquivalentTo(new Ohlc<decimal>());
            marketSnapshot.Rewards.ProviderUsd.Should().Be(0.00m);
            marketSnapshot.Rewards.MarketUsd.Should().Be(0.00m);
            marketSnapshot.SnapshotType.Should().Be(snapshotType);
            marketSnapshot.StartDate.Should().Be(date.ToStartOf(snapshotType));
            marketSnapshot.EndDate.Should().Be(date.ToEndOf(snapshotType));
        }

        // [Fact]
        // public void CreateMarketSnapshot_Success()
        // {
        //     const ulong id = 1;
        //     const ulong marketId = 2;
        //     const decimal liquidity = 234543.32m;
        //     const decimal volume = 345456.23m;
        //     var staking = new StakingSnapshot(999, 5.43m);
        //     var rewards = new RewardsSnapshot(1.42m, 5.43m);
        //     const SnapshotType snapshotType = SnapshotType.Daily;
        //     var startDate = DateTime.UtcNow.StartOfDay();
        //     var endDate = DateTime.UtcNow.EndOfDay();
        //
        //     var marketSnapshot = new MarketSnapshot(id, marketId, liquidity, volume, staking, rewards, snapshotType, startDate, endDate);
        //
        //     marketSnapshot.Id.Should().Be(id);
        //     marketSnapshot.MarketId.Should().Be(marketId);
        //     marketSnapshot.Liquidity.Should().Be(liquidity);
        //     marketSnapshot.Volume.Should().Be(volume);
        //     marketSnapshot.Staking.Usd.Should().Be(staking.Usd);
        //     marketSnapshot.Staking.Weight.Should().Be(staking.Weight);
        //     marketSnapshot.Rewards.MarketUsd.Should().Be(rewards.MarketUsd);
        //     marketSnapshot.Rewards.ProviderUsd.Should().Be(rewards.ProviderUsd);
        //     marketSnapshot.SnapshotType.Should().Be(snapshotType);
        //     marketSnapshot.StartDate.Should().Be(startDate);
        //     marketSnapshot.EndDate.Should().Be(endDate);
        // }
    }
}
