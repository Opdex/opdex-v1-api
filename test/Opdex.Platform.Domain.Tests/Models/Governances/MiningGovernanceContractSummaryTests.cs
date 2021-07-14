using FluentAssertions;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class MiningGovernanceContractSummaryTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_MiningGovernanceContractSummary_InvalidAddress_ThrowArgumentNullException(string address)
        {
            // Arrange
            // Act
            void Act() => new MiningGovernanceContractSummary(address, 10, 12, "500", 1_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(address)} must not be null or empty.");
        }

        [Fact]
        public void CreateNew_MiningGovernanceContractSummary_InvalidMiningDuration_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const ulong miningDuration = 0;

            // Act
            void Act() => new MiningGovernanceContractSummary("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", 10, 12, "500", miningDuration);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(miningDuration)} must not be greater than 0.");
        }

        [Fact]
        public void Create_MiningGovernanceNomination_Success()
        {
            // Arrange
            const string address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            const ulong nominationPeriodEnd = 1;
            const uint miningPoolsFunded = 4;
            const string miningPoolReward = "500";
            const ulong miningDuration = 100;

            // Act
            var summary = new MiningGovernanceContractSummary(address, nominationPeriodEnd, miningPoolsFunded, miningPoolReward, miningDuration);

            // Assert
            summary.Address.Should().Be(address);
            summary.NominationPeriodEnd.Should().Be(nominationPeriodEnd);
            summary.MiningPoolsFunded.Should().Be(miningPoolsFunded);
            summary.MiningPoolReward.Should().Be(miningPoolReward);
            summary.MiningDuration.Should().Be(miningDuration);
        }
    }
}