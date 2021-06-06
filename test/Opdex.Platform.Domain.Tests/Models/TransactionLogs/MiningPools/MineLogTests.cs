using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MiningPools
{
    public class MineLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void MineLog_MinerAddressNotSet_ThrowArgumentNullException(string miner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = miner;
            txLog.amount = "259502";
            txLog.totalSupply = "259543502";
            txLog.eventType = (byte)1;

            // Act
            void Act() => new MineLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void MineLog_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = amount;
            txLog.totalSupply = "259543502";
            txLog.eventType = (byte)1;

            // Act
            void Act() => new MineLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void MineLog_TotalSupplyNotValid_ThrowArgumentOutOfRangeException(string totalSupply)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = "44";
            txLog.totalSupply = totalSupply;
            txLog.eventType = (byte)1;

            // Act
            void Act() => new MineLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void MineLog_ValidArguments_PropertiesSet()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = "44";
            txLog.totalSupply = "545578999";
            txLog.eventType = (byte)1;

            // Act
            var log = new MineLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Miner.Should().Be(txLog.miner);
            log.Amount.Should().Be(txLog.amount);
            log.TotalSupply.Should().Be(txLog.totalSupply);
            log.EventType.Should().Be(txLog.eventType);
        }
    }
}