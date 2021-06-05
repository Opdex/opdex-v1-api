using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class StakeLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateStakeLog_StakerAddressNotSet_ThrowArgumentNullException(string staker)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = staker;
            txLog.amount = "4353";
            txLog.totalStaked = "39092935";
            txLog.eventType = (byte)1;

            // Act
            void Act() => new StakeLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateStakeLog_AmountIsNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = amount;
            txLog.totalStaked = "39092935";
            txLog.eventType = (byte)1;

            // Act
            void Act() => new StakeLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateStakeLog_TotalStakedIsNotValid_ThrowArgumentOutOfRangeException(string totalStaked)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "382";
            txLog.totalStaked = totalStaked;
            txLog.eventType = (byte)1;

            // Act
            void Act() => new StakeLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateStakeLog_ArgumentsValid_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "382";
            txLog.totalStaked = "593592493";
            txLog.eventType = (byte)1;

            // Act
            var log = new StakeLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Staker.Should().Be(txLog.staker);
            log.Amount.Should().Be(txLog.amount);
            log.TotalStaked.Should().Be(txLog.totalStaked);
            log.EventType.Should().Be(txLog.eventType);
        }
    }
}