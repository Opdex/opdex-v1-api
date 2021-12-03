using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MiningPools;

public class EnableMiningLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ABC")]
    [InlineData("100.005")]
    [InlineData("100_000")]
    public void EnableMiningLog_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.amount = amount;
        txLog.rewardRate = "53445";
        txLog.miningPeriodEndBlock = 50ul;

        // Act
        void Act() => new EnableMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.ThrowsAny<Exception>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ABC")]
    [InlineData("100.005")]
    [InlineData("100_000")]
    public void EnableMiningLog_RewardRateNotValid_ThrowArgumentOutOfRangeException(string rewardRate)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.amount = "43822";
        txLog.rewardRate = rewardRate;
        txLog.miningPeriodEndBlock = 50ul;

        // Act
        void Act() => new EnableMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.ThrowsAny<Exception>(Act);
    }

    [Fact]
    public void EnableMiningLog_MiningPeriodEndBlockZero_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.amount = "43822";
        txLog.rewardRate = "26";
        txLog.miningPeriodEndBlock = 0ul;

        // Act
        void Act() => new EnableMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void EnableMiningLog_ArgumentsValid_PropertiesSet()
    {
        // Arrange
        UInt256 amount = 332432;
        UInt256 rewardRate = 26;

        dynamic txLog = new ExpandoObject();
        txLog.amount = amount.ToString();
        txLog.rewardRate = rewardRate.ToString();
        txLog.miningPeriodEndBlock = 50ul;

        // Act
        var log = new EnableMiningLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.Amount.Should().Be(amount);
        log.RewardRate.Should().Be(rewardRate);
        log.MiningPeriodEndBlock.Should().Be(txLog.miningPeriodEndBlock);
    }
}