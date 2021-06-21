using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Pools.Snapshots
{
    public class StakingSnapshotTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1.234")]
        [InlineData(null)]
        public void CreateStakingSnapshot_InvalidWeight_ThrowsArgumentOutOfRangeException(string stakingWeight)
        {
            // Arrange
            // Act
            void Act() => new StakingSnapshot(stakingWeight, 1.00m);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(stakingWeight)} must be a numeric value.");
        }

        [Fact]
        public void CreateStakingSnapshot_InvalidUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal stakingUsd = -1.00m;

            // Act
            void Act() => new StakingSnapshot("1234", stakingUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(stakingUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateStakingSnapshot_Success()
        {
            // Arrange
            const string stakingWeight = "123";
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
            snapshot.Weight.Should().Be("0");
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}