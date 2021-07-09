using FluentAssertions;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class MiningGovernanceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_MiningGovernance_InvalidAddress_ThrowArgumentNullException(string address)
        {
            // Arrange
            // Act
            void Act() => new MiningGovernance(address, 10, 100, 1000, 12, "500", 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(address)} must not be null or empty.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_InvalidMiningDuration_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong miningDuration = 0;

            // Act
            void Act() => new MiningGovernance("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", 10, 100, miningDuration, 12, "500", 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(miningDuration)} must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_InvalidTokenId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long tokenId = 0;

            // Act
            void Act() => new MiningGovernance("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", tokenId, 100, 1000, 12, "500", 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(tokenId)} must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_Success()
        {
            // Arrange
            const string address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const long tokenId = 1;
            const ulong nominationPeriodEnd = 10;
            const ulong miningDuration = 100;
            const uint miningPoolsFunded = 12;
            const string miningPoolReward = "500";
            const ulong createdBlock = 150;

            // Act
            var governance = new MiningGovernance(address, tokenId, nominationPeriodEnd, miningDuration,
                                                  miningPoolsFunded, miningPoolReward, createdBlock);

            // Assert
            governance.Address.Should().Be(address);
            governance.TokenId.Should().Be(tokenId);
            governance.NominationPeriodEnd.Should().Be(nominationPeriodEnd);
            governance.MiningDuration.Should().Be(miningDuration);
            governance.MiningPoolsFunded.Should().Be(miningPoolsFunded);
            governance.MiningPoolReward.Should().Be(miningPoolReward);
            governance.CreatedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void Create_MiningGovernance_Success()
        {
            // Arrange
            const long id = 99;
            const string address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const long tokenId = 1;
            const ulong nominationPeriodEnd = 10;
            const ulong miningDuration = 100;
            const uint miningPoolsFunded = 12;
            const string miningPoolReward = "500";
            const ulong createdBlock = 150;
            const ulong modifiedBlock = 250;

            // Act
            var governance = new MiningGovernance(id, address, tokenId, nominationPeriodEnd, miningDuration,
                                                  miningPoolsFunded, miningPoolReward, createdBlock, modifiedBlock);

            // Assert
            governance.Id.Should().Be(id);
            governance.Address.Should().Be(address);
            governance.TokenId.Should().Be(tokenId);
            governance.NominationPeriodEnd.Should().Be(nominationPeriodEnd);
            governance.MiningDuration.Should().Be(miningDuration);
            governance.MiningPoolsFunded.Should().Be(miningPoolsFunded);
            governance.MiningPoolReward.Should().Be(miningPoolReward);
            governance.CreatedBlock.Should().Be(createdBlock);
            governance.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void MiningGovernance_Update_Success()
        {
            // Arrange
            const long id = 2;
            const string address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const long tokenId = 1;
            const ulong nominationPeriodEnd = 10;
            const ulong miningDuration = 100;
            const uint miningPoolsFunded = 12;
            const string miningPoolReward = "500";
            const ulong createdBlock = 150;
            var governance = new MiningGovernance(id, address, tokenId, nominationPeriodEnd, miningDuration, miningPoolsFunded,
                                                  miningPoolReward, createdBlock, createdBlock);

            var summary = new MiningGovernanceContractSummary(address, nominationPeriodEnd + 10, miningPoolsFunded + 4, "1000", 1000);

            const ulong modifiedBlock = 150;

            // Act
            governance.Update(summary, modifiedBlock);

            // Assert
            governance.NominationPeriodEnd.Should().Be(summary.NominationPeriodEnd);
            governance.MiningPoolsFunded.Should().Be(summary.MiningPoolsFunded);
            governance.MiningPoolReward.Should().Be(summary.MiningPoolReward);
            governance.MiningDuration.Should().Be(summary.MiningDuration);
            governance.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
