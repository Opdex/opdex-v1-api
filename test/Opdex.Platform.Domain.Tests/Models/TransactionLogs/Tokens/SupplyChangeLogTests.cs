using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens;

public class SupplyChangeLogTests
{
    [Fact]
    public void CreateSupplyChangeLog_ArgumentsValid_SetProperties()
    {
        // Arrange
        UInt256 previousSupply = UInt256.Zero;
        UInt256 totalSupply = 5435438952;

        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = previousSupply.ToString();
        txLog.totalSupply = totalSupply.ToString();

        // Act
        var log = new SupplyChangeLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        log.PreviousSupply.Should().Be(previousSupply);
        log.TotalSupply.Should().Be(totalSupply);
    }
}
