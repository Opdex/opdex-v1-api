using FluentAssertions;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens
{
    public class TokenDistributionTests
    {
        [Fact]
        public void CreateTokenDistribution_InvalidTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const long tokenId = 0;

            // Act
            static void Act() => new TokenDistribution(tokenId, "10", "20", 2, 3, 4, 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token Id must be greater than 0.");
        }

        [Theory]
        [InlineData("asdf")]
        [InlineData("1.11")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void CreateTokenDistribution_InvalidVaultDistribution_ThrowsArgumentException(string vaultDistribution)
        {
            // Arrange
            // Act
            void Act() => new TokenDistribution(1, vaultDistribution, "20", 2, 3, 4, 5);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Vault distribution must only contain numeric digits.");
        }

        [Theory]
        [InlineData("asdf")]
        [InlineData("1.11")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void CreateTokenDistribution_InvalidMiningGovernanceDistribution_ThrowsArgumentException(string miningGovernanceDistribution)
        {
            // Arrange
            // Act
            void Act() => new TokenDistribution(1, "10", miningGovernanceDistribution, 2, 3, 4, 5);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Mining governance distribution must only contain numeric digits.");
        }

        [Fact]
        public void CreateTokenDistribution_InvalidDistributionBlock_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong distributionBlock = 0;

            // Act
            static void Act() => new TokenDistribution(1, "10", "20", 2, distributionBlock, 4, 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Distribution block must be greater than 0.");
        }

        [Fact]
        public void CreateTokenDistribution_InvalidNextDistributionBlock_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong nextDistributionBlock = 0;

            // Act
            static void Act() => new TokenDistribution(1, "10", "20", 2, 3, nextDistributionBlock, 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Next distribution block must be greater than 0.");
        }

        [Fact]
        public void CreatesNew_TokenDistribution_Success()
        {
            // Arrange
            const long tokenId = 1;
            const string vaultDistribution = "10000000";
            const string governanceDistribution = "3000000";
            const int periodIndex = 2;
            const ulong distributionBlock = 3;
            const ulong nextDistributionBlock = 4;
            const ulong createdBlock = 5;

            // Act
            var distribution = new TokenDistribution(tokenId, vaultDistribution, governanceDistribution, periodIndex,
                                                     distributionBlock, nextDistributionBlock, createdBlock);

            // Assert
            distribution.Id.Should().Be(0);
            distribution.TokenId.Should().Be(tokenId);
            distribution.VaultDistribution.Should().Be(vaultDistribution);
            distribution.MiningGovernanceDistribution.Should().Be(governanceDistribution);
            distribution.PeriodIndex.Should().Be(periodIndex);
            distribution.DistributionBlock.Should().Be(distributionBlock);
            distribution.NextDistributionBlock.Should().Be(nextDistributionBlock);
            distribution.CreatedBlock.Should().Be(createdBlock);
            distribution.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void CreatesExisting_TokenDistribution_Success()
        {
            // Arrange
            const long id = 999;
            const long tokenId = 1;
            const string vaultDistribution = "10000000";
            const string governanceDistribution = "3000000";
            const int periodIndex = 2;
            const ulong distributionBlock = 3;
            const ulong nextDistributionBlock = 4;
            const ulong createdBlock = 5;
            const ulong modifiedBlock = 6;

            // Act
            var distribution = new TokenDistribution(id, tokenId, vaultDistribution, governanceDistribution, periodIndex,
                                                     distributionBlock, nextDistributionBlock, createdBlock, modifiedBlock);

            // Assert
            distribution.Id.Should().Be(id);
            distribution.TokenId.Should().Be(tokenId);
            distribution.VaultDistribution.Should().Be(vaultDistribution);
            distribution.MiningGovernanceDistribution.Should().Be(governanceDistribution);
            distribution.PeriodIndex.Should().Be(periodIndex);
            distribution.DistributionBlock.Should().Be(distributionBlock);
            distribution.NextDistributionBlock.Should().Be(nextDistributionBlock);
            distribution.CreatedBlock.Should().Be(createdBlock);
            distribution.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
