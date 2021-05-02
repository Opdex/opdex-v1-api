using System;
using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class MarketSnapshotTests
    {
        [Fact]
        public void CreateNewMarketSnapshot_Success()
        {
            const long marketId = 2;
            const long transactionCount = 1234;
            const decimal liquidity = 234543.32m;
            const decimal volume = 345456.23m;
            const string weight = "1235";
            const decimal weightUsd = 8765.00m; 
            const decimal providerRewards = 87654.21m;
            const decimal stakerRewards = 2334.21m;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = DateTime.UtcNow.StartOfDay();
            var endDate = DateTime.UtcNow.EndOfDay();

            var marketSnapshot = new MarketSnapshot(marketId, transactionCount, liquidity, volume, weight, weightUsd, providerRewards, stakerRewards,
                snapshotType, startDate, endDate);

            marketSnapshot.MarketId.Should().Be(marketId);
            marketSnapshot.TransactionCount.Should().Be(transactionCount);
            marketSnapshot.Liquidity.Should().Be(liquidity);
            marketSnapshot.Volume.Should().Be(volume);
            marketSnapshot.Weight.Should().Be(weight);
            marketSnapshot.WeightUsd.Should().Be(weightUsd);
            marketSnapshot.ProviderRewards.Should().Be(providerRewards);
            marketSnapshot.StakerRewards.Should().Be(stakerRewards);
            marketSnapshot.SnapshotType.Should().Be(snapshotType);
            marketSnapshot.StartDate.Should().Be(startDate);
            marketSnapshot.EndDate.Should().Be(endDate);
        }
        
        [Fact]
        public void CreateMarketSnapshot_Success()
        {
            const long id = 1;
            const long marketId = 2;
            const long transactionCount = 1234;
            const decimal liquidity = 234543.32m;
            const decimal volume = 345456.23m;
            const string weight = "1235";
            const decimal weightUsd = 8765.00m; 
            const decimal providerRewards = 87654.21m;
            const decimal stakerRewards = 2334.21m;
            const SnapshotType snapshotType = SnapshotType.Daily;
            var startDate = DateTime.UtcNow.StartOfDay();
            var endDate = DateTime.UtcNow.EndOfDay();

            var marketSnapshot = new MarketSnapshot(id, marketId, transactionCount, liquidity, volume, weight, weightUsd, providerRewards, stakerRewards,
                snapshotType, startDate, endDate);

            marketSnapshot.Id.Should().Be(id);
            marketSnapshot.MarketId.Should().Be(marketId);
            marketSnapshot.TransactionCount.Should().Be(transactionCount);
            marketSnapshot.Liquidity.Should().Be(liquidity);
            marketSnapshot.Volume.Should().Be(volume);
            marketSnapshot.Weight.Should().Be(weight);
            marketSnapshot.WeightUsd.Should().Be(weightUsd);
            marketSnapshot.ProviderRewards.Should().Be(providerRewards);
            marketSnapshot.StakerRewards.Should().Be(stakerRewards);
            marketSnapshot.SnapshotType.Should().Be(snapshotType);
            marketSnapshot.StartDate.Should().Be(startDate);
            marketSnapshot.EndDate.Should().Be(endDate);
        }
    }
}