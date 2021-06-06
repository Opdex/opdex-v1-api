using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class CollectStakingRewardsLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CollectStakingRewardsLog_StakerAddressNotSet_ThrowArgumentNullException(string staker)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = staker;
            txLog.reward = "3426893";

            // Act
            void Act() => new CollectStakingRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CollectStakingRewardsLog_RewardsNotNumeric_ThrowArgumentNullException(string reward)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.reward = reward;

            // Act
            void Act() => new CollectStakingRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CollectStakingRewardsLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.reward = "3426893";

            // Act
            var collectStakingRewardsLog = new CollectStakingRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            collectStakingRewardsLog.Staker.Should().Be(txLog.staker);
            collectStakingRewardsLog.Reward.Should().Be(txLog.reward);
        }
    }
}