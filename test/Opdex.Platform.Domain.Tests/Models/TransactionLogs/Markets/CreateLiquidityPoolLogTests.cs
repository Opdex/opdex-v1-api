using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Markets;

public class CreateLiquidityPoolLogTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateLiquidityPoolLog_TokenAddressNotSet_ThrowArgumentNullException(string token)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.token = token;
        txLog.pool = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";

        // Act
        void Act() => new CreateLiquidityPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateLiquidityPoolLog_PoolAddressNotSet_ThrowArgumentNullException(string pool)
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.token = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
        txLog.pool = pool;

        // Act
        void Act() => new CreateLiquidityPoolLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void CreatesCreateLiquidityPoolLog_Success()
    {
        Address address = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        const int sortOrder = 1;

        dynamic txLog = new ExpandoObject();
        txLog.pool = "PPGBccfFS1cKedqY5ZzJY7iaeEwpXHKzNb";
        txLog.token = "PXVmcSebfJwMY4HKm8TAiRLmi7fjYQgCwY";

        var log = new CreateLiquidityPoolLog(txLog, address, sortOrder);

        log.Id.Should().Be(0);
        log.TransactionId.Should().Be(0);
        log.LogType.Should().Be(TransactionLogType.CreateLiquidityPoolLog);
        log.Contract.Should().Be(address);
        log.SortOrder.Should().Be(sortOrder);
        log.Pool.Should().Be(txLog.pool);
        log.Token.Should().Be(txLog.token);
    }
}