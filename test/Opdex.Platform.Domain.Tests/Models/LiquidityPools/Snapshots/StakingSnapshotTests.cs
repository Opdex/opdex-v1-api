using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
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
    }
}
