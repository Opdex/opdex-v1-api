using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Markets;

public class ClaimPendingMarketOwnershipLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateClaimPendingMarketOwnershipLog_FromAddressNotSet_ThrowArgumentNullException(string from)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = from;
        txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";

        // Act
        void Act() => new ClaimPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateClaimPendingMarketOwnershipLog_ToAddressNotSet_ThrowArgumentNullException(string to)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.to = to;

        // Act
        void Act() => new ClaimPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void CreateClaimPendingMarketOwnershipLog_ArgumentsValid_SetProperties()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.to = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";

        // Act
        var log = new ClaimPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.From.Should().Be(txLog.from);
        log.To.Should().Be(txLog.to);
    }
}