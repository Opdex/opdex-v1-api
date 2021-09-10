using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class StakingSnapshotTests
    {
        [Fact]
        public void CreateStakingSnapshot_InvalidUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal stakingUsd = -1.00m;

            // Act
            void Act() => new StakingSnapshot(1234, stakingUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(stakingUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateStakingSnapshot_Success()
        {
            // Arrange
            UInt256 stakingWeight = 123;
            const decimal stakingUsd = 1.23m;

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
            snapshot.Weight.Should().Be(UInt256.Zero);
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}
