using System;
using FluentAssertions;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
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
    }
}