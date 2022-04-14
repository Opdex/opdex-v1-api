using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models;

public class OhlcTests
{
    [Fact]
    public void Creates_NewDecimal_Success()
    {
        // Arrange
        // Act
        var ohlc = new Ohlc<decimal>();

        // Assert
        ohlc.Open.Should().Be(0m);
        ohlc.High.Should().Be(0m);
        ohlc.Low.Should().Be(0m);
        ohlc.Close.Should().Be(0m);
    }

    [Fact]
    public void Creates_NewUInt256_Success()
    {
        // Arrange
        // Act
        var ohlc = new Ohlc<UInt256>();

        // Assert
        ohlc.Open.Should().Be(UInt256.Zero);
        ohlc.High.Should().Be(UInt256.Zero);
        ohlc.Low.Should().Be(UInt256.Zero);
        ohlc.Close.Should().Be(UInt256.Zero);
    }

    [Fact]
    public void Creates_NewUInt64_Success()
    {
        // Arrange
        // Act
        var ohlc = new Ohlc<ulong>();

        // Assert
        ohlc.Open.Should().Be(0ul);
        ohlc.High.Should().Be(0ul);
        ohlc.Low.Should().Be(0ul);
        ohlc.Close.Should().Be(0ul);
    }

    [Fact]
    public void Creates_ExistingDecimal_Success()
    {
        // Arrange
        const decimal open = 10m;
        const decimal high = 20m;
        const decimal low = 9m;
        const decimal close = 15m;

        // Act
        var ohlc = new Ohlc<decimal>(open, high, low, close);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(high);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(close);
    }

    [Fact]
    public void Creates_ExistingUInt256_Success()
    {
        // Arrange
        UInt256 open = 10;
        UInt256 high = 20;
        UInt256 low = 9;
        UInt256 close = 15;

        // Act
        var ohlc = new Ohlc<UInt256>(open, high, low, close);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(high);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(close);
    }

    [Fact]
    public void Creates_ExistingUInt64_Success()
    {
        // Arrange
        const ulong open = 10ul;
        const ulong high = 20ul;
        const ulong low = 9ul;
        const ulong close = 15ul;

        // Act
        var ohlc = new Ohlc<ulong>(open, high, low, close);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(high);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(close);
    }

    [Fact]
    public void Update_ExistingDecimal_Success()
    {
        // Arrange
        const decimal open = 10m;
        const decimal high = 20m;
        const decimal low = 9m;
        const decimal close = 15m;
        const decimal update = 22m;
        var ohlc = new Ohlc<decimal>(open, high, low, close);

        // Act
        ohlc.Update(update);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(update);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(update);
    }

    [Fact]
    public void Update_ExistingUInt256_Success()
    {
        // Arrange
        UInt256 open = 10;
        UInt256 high = 20;
        UInt256 low = 9;
        UInt256 close = 15;
        UInt256 update = 22;
        var ohlc = new Ohlc<UInt256>(open, high, low, close);

        // Act
        ohlc.Update(update);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(update);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(update);
    }

    [Fact]
    public void Update_ExistingUInt64_Success()
    {
        // Arrange
        const ulong open = 10ul;
        const ulong high = 20ul;
        const ulong low = 9ul;
        const ulong close = 15ul;
        const ulong update = 8ul;
        var ohlc = new Ohlc<ulong>(open, high, low, close);

        // Act
        ohlc.Update(update);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(high);
        ohlc.Low.Should().Be(update);
        ohlc.Close.Should().Be(update);
    }

    [Fact]
    public void Update_ExistingDecimal_Success_Test()
    {
        // Arrange
        const decimal open = 10m;
        const decimal high = 20m;
        const decimal low = 9m;
        const decimal close = 15m;
        const decimal update = 22m;
        var ohlc = new Ohlc<decimal>(open, high, low, close);

        // Act
        ohlc.Update(update);

        // Assert
        ohlc.Open.Should().Be(open);
        ohlc.High.Should().Be(update);
        ohlc.Low.Should().Be(low);
        ohlc.Close.Should().Be(update);
    }
}
