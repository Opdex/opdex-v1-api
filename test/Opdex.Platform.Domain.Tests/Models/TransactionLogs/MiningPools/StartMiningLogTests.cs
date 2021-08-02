using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MiningPools
{
    public class StartMiningLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void StartMiningLog_MinerAddressNotSet_ThrowArgumentNullException(string miner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = miner;
            txLog.amount = "259502";
            txLog.totalSupply = "259543502";
            txLog.minerBalance = "123";

            // Act
            void Act() => new StartMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void StartMiningLog_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = amount;
            txLog.totalSupply = "259543502";
            txLog.minerBalance = "123";

            // Act
            void Act() => new StartMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void StartMiningLog_TotalSupplyNotValid_ThrowArgumentOutOfRangeException(string totalSupply)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = "44";
            txLog.totalSupply = totalSupply;
            txLog.minerBalance = "123";

            // Act
            void Act() => new StartMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void StartMiningLog_MinerBalanceNotValid_ThrowArgumentOutOfRangeException(string minerBalance)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = "44";
            txLog.totalSupply = "12345";
            txLog.minerBalance = minerBalance;

            // Act
            void Act() => new StartMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void StartMiningLog_ValidArguments_PropertiesSet()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj4";
            txLog.amount = "44";
            txLog.totalSupply = "545578999";
            txLog.minerBalance = "100";

            // Act
            var log = new StartMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Miner.Should().Be(txLog.miner);
            log.Amount.Should().Be(txLog.amount);
            log.TotalSupply.Should().Be(txLog.totalSupply);
            log.MinerBalance.Should().Be(txLog.minerBalance);
        }
    }
}
