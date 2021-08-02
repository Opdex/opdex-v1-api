using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class StopStakingLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateStopStakingLog_StakerAddressNotSet_ThrowArgumentNullException(string staker)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = staker;
            txLog.amount = "4353";
            txLog.totalStaked = "39092935";
            txLog.stakerBalance = "100";

            // Act
            void Act() => new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateStopStakingLog_AmountIsNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = amount;
            txLog.totalStaked = "39092935";
            txLog.stakerBalance = "100";

            // Act
            void Act() => new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CreateStopStakingLog_TotalStakedIsNotValid_ThrowArgumentOutOfRangeException(string totalStaked)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "382";
            txLog.totalStaked = totalStaked;
            txLog.stakerBalance = "100";

            // Act
            void Act() => new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CreateStopStakingLog_StakerBalanceIsNotValid_ThrowArgumentOutOfRangeException(string stakerBalance)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "382";
            txLog.totalStaked = "100";
            txLog.stakerBalance = stakerBalance;

            // Act
            void Act() => new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateStopStakingLog_ArgumentsValid_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "382";
            txLog.totalStaked = "593592493";
            txLog.stakerBalance = "100";

            // Act
            var log = new StopStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Staker.Should().Be(txLog.staker);
            log.Amount.Should().Be(txLog.amount);
            log.TotalStaked.Should().Be(txLog.totalStaked);
            log.StakerBalance.Should().Be(txLog.stakerBalance);
        }
    }
}
