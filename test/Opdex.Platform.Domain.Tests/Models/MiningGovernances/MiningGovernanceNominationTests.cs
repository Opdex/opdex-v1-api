using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningGovernances
{
    public class MiningGovernanceNominationNominationTests
    {
        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidMiningGovernanceId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong miningGovernanceId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(miningGovernanceId, 1, 2, true, 100, 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining governance id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong liquidityPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(1, liquidityPoolId, 2, true, 100, 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidMiningPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong miningPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(2, 1, miningPoolId, true, 100, 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_Success()
        {
            // Arrange
            const ulong miningGovernanceId = 9;
            const ulong liquidityPoolId = 1;
            const ulong miningPoolId = 2;
            const bool isNominated = true;
            UInt256 weight = 125;
            const ulong createdBlock = 100;

            // Act
            var nomination = new MiningGovernanceNomination(miningGovernanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            // Assert
            nomination.MiningGovernanceId.Should().Be(miningGovernanceId);
            nomination.LiquidityPoolId.Should().Be(liquidityPoolId);
            nomination.MiningPoolId.Should().Be(miningPoolId);
            nomination.IsNominated.Should().Be(isNominated);
            nomination.Weight.Should().Be(weight);
            nomination.CreatedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void Create_MiningGovernanceNomination_Success()
        {
            // Arrange
            const ulong id = 1;
            const ulong miningGovernanceId = 9;
            const ulong liquidityPoolId = 1;
            const ulong miningPoolId = 2;
            const bool isNominated = true;
            UInt256 weight = 125;
            const ulong createdBlock = 100;
            const ulong modifiedBlock = 150;

            // Act
            var nomination = new MiningGovernanceNomination(id, miningGovernanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock, modifiedBlock);

            // Assert
            nomination.Id.Should().Be(id);
            nomination.MiningGovernanceId.Should().Be(miningGovernanceId);
            nomination.LiquidityPoolId.Should().Be(liquidityPoolId);
            nomination.MiningPoolId.Should().Be(miningPoolId);
            nomination.IsNominated.Should().Be(isNominated);
            nomination.Weight.Should().Be(weight);
            nomination.CreatedBlock.Should().Be(createdBlock);
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void MiningGovernanceNomination_SetStatus_Success()
        {
            // Arrange
            const ulong miningGovernanceId = 9;
            const ulong liquidityPoolId = 1;
            const ulong miningPoolId = 2;
            const bool isNominated = true;
            UInt256 weight = 125;
            const ulong createdBlock = 100;

            var nomination = new MiningGovernanceNomination(miningGovernanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            const ulong modifiedBlock = 150;

            // Act
            nomination.SetStatus(false, modifiedBlock);

            // Assert
            nomination.IsNominated.Should().BeFalse();
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void MiningGovernanceNomination_SetWeight_Success()
        {
            // Arrange
            const ulong miningGovernanceId = 9;
            const ulong liquidityPoolId = 1;
            const ulong miningPoolId = 2;
            const bool isNominated = true;
            UInt256 weight = 125;
            const ulong createdBlock = 100;
            UInt256 newWeight = 150;

            var nomination = new MiningGovernanceNomination(miningGovernanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            const ulong modifiedBlock = 150;

            // Act
            nomination.SetWeight(newWeight, modifiedBlock);

            // Assert
            nomination.Weight.Should().Be(newWeight);
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
