using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningGovernances
{
    public class MiningGovernanceTests
    {
        [Fact]
        public void CreateNew_MiningGovernance_InvalidAddress_ThrowArgumentNullException()
        {
            // Arrange
            Address address = Address.Empty;
            const ulong tokenId = 1;
            const ulong miningDuration = 2;
            const ulong createdBlock = 3;

            // Act
            void Act() => new MiningGovernance(address, tokenId, miningDuration, createdBlock);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(address)} must not be null or empty.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_InvalidMiningDuration_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong tokenId = 1;
            const ulong miningDuration = 0;
            const ulong createdBlock = 3;

            // Act
            void Act() => new MiningGovernance(address, tokenId, miningDuration, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(miningDuration)} must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_InvalidTokenId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong tokenId = 0;
            const ulong miningDuration = 2;
            const ulong createdBlock = 3;

            // Act
            void Act() => new MiningGovernance(address, tokenId, miningDuration, createdBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(tokenId)} must be greater than 0.");
        }

        [Fact]
        public void CreateNew_MiningGovernance_Success()
        {
            // Arrange
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong tokenId = 1;
            const ulong miningDuration = 100;
            const ulong createdBlock = 150;

            // Act
            var miningGovernance = new MiningGovernance(address, tokenId, miningDuration, createdBlock);

            // Assert
            miningGovernance.Address.Should().Be(address);
            miningGovernance.TokenId.Should().Be(tokenId);
            miningGovernance.MiningDuration.Should().Be(miningDuration);
            miningGovernance.CreatedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void Create_MiningGovernance_Success()
        {
            // Arrange
            const ulong id = 99ul;
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong tokenId = 1;
            const ulong nominationPeriodEnd = 10;
            const ulong miningDuration = 100;
            const uint miningPoolsFunded = 12;
            UInt256 miningPoolReward = 500;
            const ulong createdBlock = 150;
            const ulong modifiedBlock = 250;

            // Act
            var miningGovernance = new MiningGovernance(id, address, tokenId, nominationPeriodEnd, miningDuration,
                                                  miningPoolsFunded, miningPoolReward, createdBlock, modifiedBlock);

            // Assert
            miningGovernance.Id.Should().Be(id);
            miningGovernance.Address.Should().Be(address);
            miningGovernance.TokenId.Should().Be(tokenId);
            miningGovernance.NominationPeriodEnd.Should().Be(nominationPeriodEnd);
            miningGovernance.MiningDuration.Should().Be(miningDuration);
            miningGovernance.MiningPoolsFunded.Should().Be(miningPoolsFunded);
            miningGovernance.MiningPoolReward.Should().Be(miningPoolReward);
            miningGovernance.CreatedBlock.Should().Be(createdBlock);
            miningGovernance.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void MiningGovernance_Update_Success()
        {
            // Arrange
            const ulong id = 2;
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong tokenId = 1;
            const ulong nominationPeriodEnd = 10;
            const ulong miningDuration = 100;
            const uint miningPoolsFunded = 12;
            UInt256 miningPoolReward = 500;
            const ulong createdBlock = 150;

            var miningGovernance = new MiningGovernance(id, address, tokenId, nominationPeriodEnd, miningDuration, miningPoolsFunded,
                                                  miningPoolReward, createdBlock, createdBlock);

            const ulong modifiedBlock = 150;
            var summary = new MiningGovernanceContractSummary(modifiedBlock);
            summary.SetMiningDuration(new SmartContractMethodParameter(200ul));
            summary.SetMiningPoolReward(new SmartContractMethodParameter(new UInt256("500")));
            summary.SetMiningPoolsFunded(new SmartContractMethodParameter((uint)20));
            summary.SetMinedToken(new SmartContractMethodParameter(new Address("Pq2KGFTtoSPNG9Xh2WU8q87nBDE7FiEUa8")));
            summary.SetNominationPeriodEnd(new SmartContractMethodParameter(1000ul));

            // Act
            miningGovernance.Update(summary);

            // Assert
            miningGovernance.NominationPeriodEnd.Should().Be(summary.NominationPeriodEnd);
            miningGovernance.MiningPoolsFunded.Should().Be(summary.MiningPoolsFunded);
            miningGovernance.MiningPoolReward.Should().Be(summary.MiningPoolReward);
            miningGovernance.MiningDuration.Should().Be(miningDuration);
            miningGovernance.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
