using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens
{
    public class DistributionLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void DistributionLog_VaultAmountNotValid_ThrowArgumentOutOfRangeException(string vaultAmount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.vaultAmount = vaultAmount;
            txLog.miningAmount = "259543502";
            txLog.periodIndex = 1u;

            // Act
            void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void DistributionLog_MiningAmountNotValid_ThrowArgumentOutOfRangeException(string miningAmount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.vaultAmount = "43299992";
            txLog.miningAmount = miningAmount;
            txLog.periodIndex = 1u;

            // Act
            void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void DistributionLog_ValidArguments_PropertiesSet()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.vaultAmount = "43299992";
            txLog.miningAmount = "13295";
            txLog.periodIndex = 1u;

            // Act
            var log = new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.VaultAmount.Should().Be(txLog.vaultAmount);
            log.MiningAmount.Should().Be(txLog.miningAmount);
            log.PeriodIndex.Should().Be(txLog.periodIndex);
        }
    }
}