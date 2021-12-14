using FluentAssertions;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Extensions;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Extensions;

public class TransactionErrorsTests
{
    [Fact]
    public void Opdex_TryParseFriendlyErrorMessage_Unknown()
    {
        // Arrange
        var error = @"Something went really wrong!!!";

        // Act
        var success = TransactionErrors.Opdex.TryParseFriendlyErrorMessage(error, out var friendlyMessage);

        // Assert
        success.Should().Be(false);
        friendlyMessage.Should().Be(null);
    }

    [Fact]
    public void General_TryParseFriendlyErrorMessageWindows_Overflow()
    {
        // Arrange
        var error = "System.OverflowException: Value was either too large or too small for a UInt64.\r\n   at System.Numerics.BigInteger.op_Explicit(BigInteger value)\r\n   at Stratis.SmartContracts.UInt256.op_Explicit(UInt256 value)\r\n   at OpdexRouter.GetAmountIn(UInt256 tokenOutAmount, UInt256 tokenOutReserveCrs, UInt256 tokenOutReserveSrc, UInt256 tokenInReserveCrs, UInt256 tokenInReserveSrc)";

        // Act
        var success = TransactionErrors.Opdex.TryParseFriendlyErrorMessage(error, out var friendlyMessage);

        // Assert
        success.Should().Be(true);
        friendlyMessage.Should().Be("Value overflow.");
    }

    [Fact]
    public void General_TryParseFriendlyErrorMessageUnix_Overflow()
    {
        // Arrange
        var error = "System.OverflowException: Value was either too large or too small for a UInt64.\n   at System.Numerics.BigInteger.op_Explicit(BigInteger value)\n   at Stratis.SmartContracts.UInt256.op_Explicit(UInt256 value)\n   at OpdexRouter.GetAmountIn(UInt256 tokenOutAmount, UInt256 tokenOutReserveCrs, UInt256 tokenOutReserveSrc, UInt256 tokenInReserveCrs, UInt256 tokenInReserveSrc)";

        // Act
        var success = TransactionErrors.Opdex.TryParseFriendlyErrorMessage(error, out var friendlyMessage);

        // Assert
        success.Should().Be(true);
        friendlyMessage.Should().Be("Value overflow.");
    }

    [Fact]
    public void Opdex_TryParseFriendlyErrorMessageWindows_Known()
    {
        // Arrange
        var error = "Stratis.SmartContracts.SmartContractAssertException: OPDEX: INSUFFICIENT_B_AMOUNT\r\n   at Stratis.SmartContracts.SmartContract.Assert(Boolean condition, String message)\r\n   at OpdexRouter.CalculateLiquidityAmounts(Address pool, UInt64 amountCrsDesired, UInt256 amountSrcDesired, UInt64 amountCrsMin, UInt256 amountSrcMin)\r\n   at OpdexRouter.AddLiquidity(Address token, UInt256 amountSrcDesired, UInt64 amountCrsMin, UInt256 amountSrcMin, Address to, UInt64 deadline)";

        // Act
        var success = TransactionErrors.Opdex.TryParseFriendlyErrorMessage(error, out var _);

        // Assert
        success.Should().Be(true);
    }

    [Fact]
    public void Opdex_TryParseFriendlyErrorMessageUnix_Known()
    {
        // Arrange
        var error = "Stratis.SmartContracts.SmartContractAssertException: OPDEX: INSUFFICIENT_B_AMOUNT\n   at Stratis.SmartContracts.SmartContract.Assert(Boolean condition, String message)\n   at OpdexRouter.CalculateLiquidityAmounts(Address pool, UInt64 amountCrsDesired, UInt256 amountSrcDesired, UInt64 amountCrsMin, UInt256 amountSrcMin)\n   at OpdexRouter.AddLiquidity(Address token, UInt256 amountSrcDesired, UInt64 amountCrsMin, UInt256 amountSrcMin, Address to, UInt64 deadline)";

        // Act
        var success = TransactionErrors.Opdex.TryParseFriendlyErrorMessage(error, out var _);

        // Assert
        success.Should().Be(true);
    }
}
