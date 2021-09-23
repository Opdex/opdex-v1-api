using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningPools
{
    public class MiningPoolContractSummaryTests
    {
        [Fact]
        public void CreateNew_MiningPoolContractSummary_InvalidBlockHeight_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new MiningPoolContractSummary(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public void Create_MiningPoolContractSummary_Success()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            var summary = new MiningPoolContractSummary(blockHeight);

            // Assert
            summary.BlockHeight.Should().Be(blockHeight);
        }

        [Fact]
        public void MiningPoolContractSummary_SetRewardPerToken_Success()
        {
            // Arrange
            const ulong blockHeight = 10;
            var summary = new MiningPoolContractSummary(blockHeight);

            UInt256 reward = 100;

            // Act
            summary.SetRewardPerLpt(reward);

            // Assert
            summary.RewardPerLpt.Should().Be(reward);
        }

        [Fact]
        public void MiningPoolContractSummary_InvalidRewardRateType_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong blockHeight = 10;
            var summary = new MiningPoolContractSummary(blockHeight);

            UInt256 reward = 100;
            const SmartContractParameterType incorrectType = SmartContractParameterType.Address;

            // Act
            void Act() => summary.SetRewardRate(new SmartContractMethodParameter(reward.ToString(), incorrectType));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Reward rate value must be of UInt256 type.");
        }

        [Fact]
        public void MiningPoolContractSummary_SetRewardRate_Success()
        {
            // Arrange
            const ulong blockHeight = 10;
            var summary = new MiningPoolContractSummary(blockHeight);

            UInt256 reward = 100;

            // Act
            summary.SetRewardRate(new SmartContractMethodParameter(reward));

            // Assert
            summary.RewardRate.Should().Be(reward);
        }

        [Fact]
        public void MiningPoolContractSummary_InvalidMiningPeriodEndType_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong blockHeight = 10;
            var summary = new MiningPoolContractSummary(blockHeight);

            const ulong miningPeriodEnd = 100;
            const SmartContractParameterType incorrectType = SmartContractParameterType.Address;

            // Act
            void Act() => summary.SetMiningPeriodEnd(new SmartContractMethodParameter(miningPeriodEnd.ToString(), incorrectType));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining period end value must be of ulong type.");
        }

        [Fact]
        public void MiningPoolContractSummary_SetMiningPeriodEnd_Success()
        {
            // Arrange
            const ulong blockHeight = 10;
            var summary = new MiningPoolContractSummary(blockHeight);

            const ulong miningPeriodEnd = 100;

            // Act
            summary.SetMiningPeriodEnd(new SmartContractMethodParameter(miningPeriodEnd));

            // Assert
            summary.MiningPeriodEnd.Should().Be(miningPeriodEnd);
        }
    }
}
