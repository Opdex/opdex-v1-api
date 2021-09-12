using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Governances
{
    public class NominationLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateNominationLog_StakingPoolAddressNotSet_ThrowArgumentNullException(string stakingPool)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.stakingPool = stakingPool;
            txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.weight = "259502";

            // Act
            void Act() => new NominationLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateNominationLog_MiningPoolAddressNotSet_ThrowArgumentNullException(string miningPool)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.miningPool = miningPool;
            txLog.weight = "259502";

            // Act
            void Act() => new NominationLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateNominationLog_WeightNotValid_ThrowArgumentOutOfRangeException(string weight)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.weight = weight;

            // Act
            void Act() => new NominationLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.ThrowsAny<Exception>(Act);
        }

        [Fact]
        public void NominationLog_ValidArguments_SetProperties()
        {
            // Arrange
            UInt256 weight = 32853464;
            dynamic txLog = new ExpandoObject();
            txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.weight = weight.ToString();

            // Act
            var log = new NominationLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.StakingPool.Should().Be(txLog.stakingPool);
            log.MiningPool.Should().Be(txLog.miningPool);
            log.Weight.Should().Be(weight);
        }
    }
}
