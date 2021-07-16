using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    // public class ChangeMarketLogTests
    // {
    //     [Theory]
    //     [InlineData(null)]
    //     [InlineData("")]
    //     [InlineData("  ")]
    //     public void CreateChangeMarketLog_FromAddressNotSet_ThrowArgumentNullException(string from)
    //     {
    //         // Arrange
    //         dynamic txLog = new ExpandoObject();
    //         txLog.from = from;
    //         txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
    //
    //         // Act
    //         void Act() => new ChangeMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
    //
    //         // Assert
    //         Assert.Throws<ArgumentNullException>(Act);
    //     }
    //
    //     [Theory]
    //     [InlineData(null)]
    //     [InlineData("")]
    //     [InlineData("  ")]
    //     public void CreateChangeMarketLog_ToAddressNotSet_ThrowArgumentNullException(string to)
    //     {
    //         // Arrange
    //         dynamic txLog = new ExpandoObject();
    //         txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
    //         txLog.to = to;
    //
    //         // Act
    //         void Act() => new ChangeMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
    //
    //         // Assert
    //         Assert.Throws<ArgumentNullException>(Act);
    //     }
    //
    //     [Fact]
    //     public void CreateChangeMarketLog_ArgumentsValid_SetProperties()
    //     {
    //         // Arrange
    //         dynamic txLog = new ExpandoObject();
    //         txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
    //         txLog.to = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
    //
    //         // Act
    //         var changeMarketLog = new ChangeMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
    //
    //         // Assert
    //         changeMarketLog.From.Should().Be(txLog.from);
    //         changeMarketLog.To.Should().Be(txLog.to);
    //     }
    // }
}
