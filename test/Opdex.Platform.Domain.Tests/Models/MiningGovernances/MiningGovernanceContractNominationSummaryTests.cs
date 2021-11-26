using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningGovernances
{
    public class MiningGovernanceContractNominationSummaryTests
    {
        [Fact]
        public void MiningGovernanceContractNominationSummary_InvalidStakingPool_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new MiningGovernanceContractNominationSummary(null, 1_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool address must be provided.");
        }

        [Fact]
        public void MiningGovernanceContractNominationSummary_Creates()
        {
            // Arrange
            Address liquidityPool = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            UInt256 weight = 1_000;

            // Act
            var model = new MiningGovernanceContractNominationSummary(liquidityPool, weight);

            // Assert
            model.LiquidityPool.Should().Be(liquidityPool);
            model.StakingWeight.Should().Be(weight);
        }
    }
}
