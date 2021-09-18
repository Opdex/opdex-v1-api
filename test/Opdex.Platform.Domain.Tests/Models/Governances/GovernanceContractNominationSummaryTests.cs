using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Governances
{
    public class GovernanceContractNominationSummaryTests
    {
        [Fact]
        public void GovernanceContractNominationSummary_InvalidStakingPool_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new GovernanceContractNominationSummary(null, 1_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool address must be provided.");
        }

        [Fact]
        public void GovernanceContractNominationSummary_Creates()
        {
            // Arrange
            Address liquidityPool = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            UInt256 weight = 1_000;

            // Act
            var model = new GovernanceContractNominationSummary(liquidityPool, weight);

            // Assert
            model.LiquidityPool.Should().Be(liquidityPool);
            model.StakingWeight.Should().Be(weight);
        }
    }
}
