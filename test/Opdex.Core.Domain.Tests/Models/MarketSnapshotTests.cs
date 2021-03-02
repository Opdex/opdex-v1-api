using FluentAssertions;
using Opdex.Core.Domain.Models;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models
{
    public class MarketSnapshotTests
    {
        [Fact]
        public void CreateNewMarketSnapshot_Success()
        {
            const long tokenCount = 2;
            const long pairCount = 2;
            const long dailyTransactionCount = 1234;
            const decimal crsPrice = 1.11m;
            const decimal liquidity = 234543.32m;
            const decimal dailyFees = 87654.21m;
            const decimal dailyVolume = 345456.23m;
            const ulong block = 1234;

            var marketSnapshot = new MarketSnapshot(tokenCount, pairCount, dailyTransactionCount, crsPrice, liquidity,
                dailyFees, dailyVolume, block);

            marketSnapshot.TokenCount.Should().Be(tokenCount);
            marketSnapshot.PairCount.Should().Be(pairCount);
            marketSnapshot.DailyTransactionCount.Should().Be(dailyTransactionCount);
            marketSnapshot.CrsPrice.Should().Be(crsPrice);
            marketSnapshot.Liquidity.Should().Be(liquidity);
            marketSnapshot.DailyFees.Should().Be(dailyFees);
            marketSnapshot.DailyVolume.Should().Be(dailyVolume);
            marketSnapshot.Block.Should().Be(block);
        }
        
        [Fact]
        public void CreateMarketSnapshot_Success()
        {
            const long id = 1;
            const long tokenCount = 2;
            const long pairCount = 2;
            const long dailyTransactionCount = 1234;
            const decimal crsPrice = 1.11m;
            const decimal liquidity = 234543.32m;
            const decimal dailyFees = 87654.21m;
            const decimal dailyVolume = 345456.23m;
            const ulong block = 1234;

            var marketSnapshot = new MarketSnapshot(id, tokenCount, pairCount, dailyTransactionCount, crsPrice, liquidity,
                dailyFees, dailyVolume, block);

            marketSnapshot.Id.Should().Be(id);
            marketSnapshot.TokenCount.Should().Be(tokenCount);
            marketSnapshot.PairCount.Should().Be(pairCount);
            marketSnapshot.DailyTransactionCount.Should().Be(dailyTransactionCount);
            marketSnapshot.CrsPrice.Should().Be(crsPrice);
            marketSnapshot.Liquidity.Should().Be(liquidity);
            marketSnapshot.DailyFees.Should().Be(dailyFees);
            marketSnapshot.DailyVolume.Should().Be(dailyVolume);
            marketSnapshot.Block.Should().Be(block);
        }
    }
}