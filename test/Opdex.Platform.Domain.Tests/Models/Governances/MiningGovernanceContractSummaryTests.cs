using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class MiningGovernanceContractSummaryTests
    {
        [Fact]
        public void CreateNew_MiningGovernanceContractSummary_InvalidAddress_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new MiningGovernanceContractSummary(null, "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", 10, 12, 500, 1_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"Governance address must be provided.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceContractSummary_InvalidMinedToken_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new MiningGovernanceContractSummary("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", null, 10, 12, 500, 1_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Governance mined token address must be provided.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceContractSummary_InvalidMiningDuration_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong miningDuration = 0;

            // Act
            void Act() => new MiningGovernanceContractSummary("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", 10, 12, 500, miningDuration);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining duration must be greater than zero.");
        }

        [Fact]
        public void Create_MiningGovernanceNomination_Success()
        {
            // Arrange
            Address address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            Address governanceToken = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong nominationPeriodEnd = 1;
            const uint miningPoolsFunded = 4;
            UInt256 miningPoolReward = 500;
            const ulong miningDuration = 100;

            // Act
            var summary = new MiningGovernanceContractSummary(address, governanceToken, nominationPeriodEnd, miningPoolsFunded, miningPoolReward, miningDuration);

            // Assert
            summary.Address.Should().Be(address);
            summary.MinedToken.Should().Be(governanceToken);
            summary.NominationPeriodEnd.Should().Be(nominationPeriodEnd);
            summary.MiningPoolsFunded.Should().Be(miningPoolsFunded);
            summary.MiningPoolReward.Should().Be(miningPoolReward);
            summary.MiningDuration.Should().Be(miningDuration);
        }
    }
}
