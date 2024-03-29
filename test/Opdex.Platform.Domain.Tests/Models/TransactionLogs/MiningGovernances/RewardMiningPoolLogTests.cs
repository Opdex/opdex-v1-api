using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MiningGovernances;

public class RewardMiningPoolLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateRewardMiningPoolLog_StakingPoolAddressNotSet_ThrowArgumentNullException(string stakingPool)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.stakingPool = stakingPool;
        txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
        txLog.amount = "259502";

        // Act
        void Act() => new RewardMiningPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateRewardMiningPoolLog_MiningPoolAddressNotSet_ThrowArgumentNullException(string miningPool)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
        txLog.miningPool = miningPool;
        txLog.amount = "259502";

        // Act
        void Act() => new RewardMiningPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void CreateRewardMiningPoolLog_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
        txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
        txLog.amount = amount;

        // Act
        void Act() => new RewardMiningPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.ThrowsAny<Exception>(Act);
    }

    [Fact]
    public void RewardMiningPoolLog_ValidArguments_SetProperties()
    {
        // Arrange
        UInt256 amount = 32853464;

        dynamic txLog = new ExpandoObject();
        txLog.stakingPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
        txLog.miningPool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
        txLog.amount = amount.ToString();

        // Act
        var log = new RewardMiningPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.StakingPool.Should().Be(txLog.stakingPool);
        log.MiningPool.Should().Be(txLog.miningPool);
        log.Amount.Should().Be(amount);
    }
}