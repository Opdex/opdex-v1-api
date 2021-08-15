using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools
{
    public class LiquidityPoolSummaryTests
    {
        [Fact]
        public void CreateNew_LiquidityPoolSummary_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 0;
            const decimal liquidity = 2.00m;
            const decimal volume = 3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;

            // Act
            void Act() => new LiquidityPoolSummary(liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("LiquidityPoolId must be greater than 0.");
        }

        [Fact]
        public void CreateNew_LiquidityPoolSummary_InvalidLiquidity_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const decimal liquidity = -2.00m;
            const decimal volume = 3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;

            // Act
            void Act() => new LiquidityPoolSummary(liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity must be greater than or equal to 0.");
        }

        [Fact]
        public void CreateNew_LiquidityPoolSummary_InvalidVolume_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const decimal liquidity = 2.00m;
            const decimal volume = -3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;

            // Act
            void Act() => new LiquidityPoolSummary(liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Volume must be greater than or equal to 0.");
        }

        [Fact]
        public void CreateNew_LiquidityPoolSummary_Success()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const decimal liquidity = 2.00m;
            const decimal volume = 3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;

            // Act
            var result = new LiquidityPoolSummary(liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock);

            // Assert
            result.Id.Should().Be(0);
            result.LiquidityPoolId.Should().Be(liquidityPoolId);
            result.Liquidity.Should().Be(liquidity);
            result.Volume.Should().Be(volume);
            result.StakingWeight.Should().Be(stakingWeight);
            result.LockedCrs.Should().Be(lockedCrs);
            result.LockedSrc.Should().Be(lockedSrc);
            result.CreatedBlock.Should().Be(createdBlock);
            result.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void CreateExistingFromEntity_LiquidityPoolSummary_Success()
        {
            // Arrange
            const long id = 10;
            const long liquidityPoolId = 1;
            const decimal liquidity = 2.00m;
            const decimal volume = 3.00m;
            const ulong stakingWeight = 4;
            const ulong lockedCrs = 5;
            UInt256 lockedSrc = 6;
            const ulong createdBlock = 7;
            const ulong modifiedBlock = 8;

            // Act
            var result = new LiquidityPoolSummary(id, liquidityPoolId, liquidity, volume, stakingWeight, lockedCrs, lockedSrc, createdBlock, modifiedBlock);

            // Assert
            result.Id.Should().Be(id);
            result.LiquidityPoolId.Should().Be(liquidityPoolId);
            result.Liquidity.Should().Be(liquidity);
            result.Volume.Should().Be(volume);
            result.StakingWeight.Should().Be(stakingWeight);
            result.LockedCrs.Should().Be(lockedCrs);
            result.LockedSrc.Should().Be(lockedSrc);
            result.CreatedBlock.Should().Be(createdBlock);
            result.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
