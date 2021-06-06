using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MiningPools
{
    public class CollectMiningRewardsLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateCollectMiningRewardsLog_MinerAddressNotSet_ThrowArgumentNullException(string miner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = miner;
            txLog.amount = "259502";

            // Act
            void Act() => new CollectMiningRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateCollectMiningRewardsLog_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.amount = amount;

            // Act
            void Act() => new CollectMiningRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CollectMiningRewardsLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.miner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.amount = "234091833";

            // Act
            var log = new CollectMiningRewardsLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Miner.Should().Be(txLog.miner);
            log.Amount.Should().Be(txLog.amount);
        }
    }
}