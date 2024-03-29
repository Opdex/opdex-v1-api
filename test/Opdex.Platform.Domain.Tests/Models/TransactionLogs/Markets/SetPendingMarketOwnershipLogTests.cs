using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Markets;

public class SetPendingMarketOwnershipLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateSetPendingMarketOwnershipLog_FromAddressNotSet_ThrowArgumentNullException(string from)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = from;
        txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";

        // Act
        void Act() => new SetPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateSetPendingMarketOwnershipLog_ToAddressNotSet_ThrowArgumentNullException(string to)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.to = to;

        // Act
        void Act() => new SetPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void CreateSetPendingMarketOwnershipLog_ArgumentsValid_SetProperties()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.to = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";

        // Act
        var log = new SetPendingMarketOwnershipLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.From.Should().Be(txLog.from);
        log.To.Should().Be(txLog.to);
    }
}