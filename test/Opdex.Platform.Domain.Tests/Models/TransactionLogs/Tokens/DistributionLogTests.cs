using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens;

public class DistributionLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ABC")]
    [InlineData("100.005")]
    [InlineData("100_000")]
    public void DistributionLog_VaultAmountNotValid_ThrowException(string vaultAmount)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.vaultAmount = vaultAmount;
        txLog.miningAmount = "259543502";
        txLog.periodIndex = 1u;
        txLog.totalSupply = "100";
        txLog.nextDistributionBlock = 100ul;

        // Act
        void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void DistributionLog_MiningAmountNotValid_ThrowException(string miningAmount)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.vaultAmount = "43299992";
        txLog.miningAmount = miningAmount;
        txLog.periodIndex = 1u;
        txLog.totalSupply = "100";
        txLog.nextDistributionBlock = 100ul;

        // Act
        void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
    public void DistributionLog_TotalSupplyNotValid_ThrowException(string totalSupply)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.vaultAmount = "43299992";
        txLog.miningAmount = "234234";
        txLog.periodIndex = 1u;
        txLog.totalSupply = totalSupply;
        txLog.nextDistributionBlock = 100ul;

        // Act
        void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.ThrowsAny<Exception>(Act);
    }

    [Fact]
    public void DistributionLog_NextDistributionBlockNotValid_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.vaultAmount = "43299992";
        txLog.miningAmount = "234234";
        txLog.periodIndex = 1u;
        txLog.totalSupply = "100";
        txLog.nextDistributionBlock = 0ul;

        // Act
        void Act() => new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void DistributionLog_ValidArguments_PropertiesSet()
    {
        // Arrange
        UInt256 vaultAmount = 43299992;
        UInt256 miningAmount = 13295;
        UInt256 totalSupply = 100;

        dynamic txLog = new ExpandoObject();
        txLog.vaultAmount = vaultAmount.ToString();
        txLog.miningAmount = miningAmount.ToString();
        txLog.periodIndex = 1u;
        txLog.totalSupply = totalSupply.ToString();
        txLog.nextDistributionBlock = 100ul;

        // Act
        var log = new DistributionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.VaultAmount.Should().Be(vaultAmount);
        log.MiningAmount.Should().Be(miningAmount);
        log.TotalSupply.Should().Be(totalSupply);
        log.PeriodIndex.Should().Be(txLog.periodIndex);
    }
}