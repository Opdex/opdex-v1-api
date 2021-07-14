using FluentAssertions;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class MiningGovernanceNominationNominationTests
    {
        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(liquidityPoolId, 2, true, "100", 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidMiningPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long miningPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(1, miningPoolId, true, "100", 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidWeight_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const string weight = "1.25";

            // Act
            void Act() => new MiningGovernanceNomination(1, 2, true, weight, 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Weight must only contain numeric digits.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_Success()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;

            // Act
            var nomination = new MiningGovernanceNomination(liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            // Assert
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
            const long id = 1;
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;
            const ulong modifiedBlock = 150;

            // Act
            var nomination = new MiningGovernanceNomination(id, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock, modifiedBlock);

            // Assert
            nomination.Id.Should().Be(id);
            nomination.LiquidityPoolId.Should().Be(liquidityPoolId);
            nomination.MiningPoolId.Should().Be(miningPoolId);
            nomination.IsNominated.Should().Be(isNominated);
            nomination.Weight.Should().Be(weight);
            nomination.CreatedBlock.Should().Be(createdBlock);
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void MiningGovernanceNomination_SetNominationStatus_Success()
        {
            // Arrange
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;

            var nomination = new MiningGovernanceNomination(liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            const ulong modifiedBlock = 150;

            // Act
            nomination.SetNominationStatus(false, modifiedBlock);

            // Assert
            nomination.IsNominated.Should().BeFalse();
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}