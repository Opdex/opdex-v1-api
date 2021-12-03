using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools;

public class StartStakingLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateStartStakingLog_StakerAddressNotSet_ThrowArgumentNullException(string staker)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.staker = staker;
        txLog.amount = "4353";
        txLog.totalStaked = "39092935";
        txLog.stakerBalance = "100";

        // Act
        void Act() => new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void CreateStartStakingLog_AmountIsNotValid_ThrowArgumentOutOfRangeException(string amount)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = amount;
        txLog.totalStaked = "39092935";
        txLog.stakerBalance = "100";

        // Act
        void Act() => new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void CreateStartStakingLog_TotalStakedIsNotValid_ThrowArgumentOutOfRangeException(string totalStaked)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = "382";
        txLog.totalStaked = totalStaked;
        txLog.stakerBalance = "100";

        // Act
        void Act() => new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void CreateStartStakingLog_StakerBalanceIsNotValid_ThrowArgumentOutOfRangeException(string stakerBalance)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = "382";
        txLog.totalStaked = "100";
        txLog.stakerBalance = stakerBalance;

        // Act
        void Act() => new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.ThrowsAny<Exception>(Act);
    }

    [Fact]
    public void CreateStartStakingLog_ArgumentsValid_SetProperties()
    {
        // Arrange
        UInt256 amount = 453443;
        UInt256 totalStaked = 453443;
        UInt256 stakerBalance = 453443;

        dynamic txLog = new ExpandoObject();
        txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amount = amount.ToString();
        txLog.totalStaked = totalStaked.ToString();
        txLog.stakerBalance = stakerBalance.ToString();

        // Act
        var log = new StartStakingLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.Staker.Should().Be(txLog.staker);
        log.Amount.Should().Be(amount);
        log.TotalStaked.Should().Be(totalStaked);
        log.StakerBalance.Should().Be(stakerBalance);
    }
}