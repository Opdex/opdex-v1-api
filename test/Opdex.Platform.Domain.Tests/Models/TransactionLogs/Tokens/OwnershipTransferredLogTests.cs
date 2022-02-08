using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens;

public class OwnershipTransferredLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateOwnershipTransferredLog_FromAddressNotSet_ThrowArgumentNullException(string previousOwner)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = previousOwner;
        txLog.newOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";

        // Act
        void Act() => new OwnershipTransferredLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateOwnershipTransferredLog_ToAddressNotSet_ThrowArgumentNullException(string newOwner)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = newOwner;

        // Act
        void Act() => new OwnershipTransferredLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void CreateOwnershipTransferredLog_ArgumentsValid_SetProperties()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";

        // Act
        var log = new OwnershipTransferredLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.From.Should().Be(txLog.previousOwner);
        log.To.Should().Be(txLog.newOwner);
    }
}
