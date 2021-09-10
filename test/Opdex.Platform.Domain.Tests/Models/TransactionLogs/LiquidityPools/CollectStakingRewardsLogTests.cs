using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
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
            txLog.amount = "3426893";

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
        public void CollectStakingRewardsLog_RewardsNotNumeric_ThrowArgumentNullException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = amount;

            // Act
            void Act() => new CollectStakingRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.ThrowsAny<Exception>(Act);
        }

        [Fact]
        public void CollectStakingRewardsLog_ValidArguments_SetProperties()
        {
            // Arrange
            UInt256 amount = 545345;
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = amount.ToString();

            // Act
            var collectStakingRewardsLog = new CollectStakingRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            collectStakingRewardsLog.Staker.Should().Be(txLog.staker);
            collectStakingRewardsLog.Amount.Should().Be(amount);
        }
    }
}
