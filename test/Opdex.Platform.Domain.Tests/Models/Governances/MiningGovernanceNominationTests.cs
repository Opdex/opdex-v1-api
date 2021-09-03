using FluentAssertions;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class MiningGovernanceNominationNominationTests
    {
        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidGovernanceId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long governanceId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(governanceId, 1, 2, true, "100", 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Governance id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidLiquidityPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long liquidityPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(1, liquidityPoolId, 2, true, "100", 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidMiningPoolId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long miningPoolId = 0;

            // Act
            void Act() => new MiningGovernanceNomination(2, 1, miningPoolId, true, "100", 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining pool id must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_InvalidWeight_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const string weight = "1.25";

            // Act
            void Act() => new MiningGovernanceNomination(1, 1, 2, true, weight, 100);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Weight must only contain numeric digits.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceNomination_Success()
        {
            // Arrange
            const long governanceId = 9;
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;

            // Act
            var nomination = new MiningGovernanceNomination(governanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            // Assert
            nomination.GovernanceId.Should().Be(governanceId);
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
            const long governanceId = 9;
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;
            const ulong modifiedBlock = 150;

            // Act
            var nomination = new MiningGovernanceNomination(id, governanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock, modifiedBlock);

            // Assert
            nomination.Id.Should().Be(id);
            nomination.GovernanceId.Should().Be(governanceId);
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
            const long governanceId = 9;
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;

            var nomination = new MiningGovernanceNomination(governanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

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
            const long governanceId = 9;
            const long liquidityPoolId = 1;
            const long miningPoolId = 2;
            const bool isNominated = true;
            const string weight = "125";
            const ulong createdBlock = 100;
            const string newWeight = "150";

            var nomination = new MiningGovernanceNomination(governanceId, liquidityPoolId, miningPoolId, isNominated, weight, createdBlock);

            const ulong modifiedBlock = 150;

            // Act
            nomination.SetWeight(newWeight, modifiedBlock);

            // Assert
            nomination.Weight.Should().Be(newWeight);
            nomination.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
