using FluentAssertions;
using Nethereum.Util;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens;

public class TokenWrappedTests
{
    [Fact]
    public void TokenWrapped_EmptyEthereumAddress_ValidateFalse()
    {
        // Arrange
        // Act
        var wrappedToken = new TokenWrapped(5, new Address("PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT"),
            ExternalChainType.Ethereum, AddressUtil.AddressEmptyAsHex, 5000);

        // Assert
        wrappedToken.Validated.Should().Be(false);
    }

    [Fact]
    public void TokenWrapped_InvalidEthereumAddress_ValidateFalse()
    {
        // Arrange
        // Act
        var wrappedToken = new TokenWrapped(5, new Address("PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT"),
            ExternalChainType.Ethereum, "Invalid Ethereum Address", 5000);

        // Assert
        wrappedToken.Validated.Should().Be(false);
    }

    [Theory]
    [InlineData("0x514910771AF9Ca656af840dff83E8264EcF986CA")]
    [InlineData("0x514910771af9ca656af840dff83e8264ecf986ca")]
    [InlineData("514910771AF9Ca656af840dff83E8264EcF986CA")]
    [InlineData("514910771af9ca656af840dff83e8264ecf986ca")]
    public void TokenWrapped_ValidEthereumAddress_ValidateTrue(string ethereumAddress)
    {
        // Arrange
        // Act
        var wrappedToken = new TokenWrapped(5, new Address("PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT"),
            ExternalChainType.Ethereum, ethereumAddress, 5000);

        // Assert
        wrappedToken.Validated.Should().Be(true);
    }

    [Theory]
    [InlineData("0x514910771AF9Ca656af840dff83E8264EcF986CA")]
    [InlineData("0x514910771af9ca656af840dff83e8264ecf986ca")]
    [InlineData("514910771AF9Ca656af840dff83E8264EcF986CA")]
    [InlineData("514910771af9ca656af840dff83e8264ecf986ca")]
    public void TokenWrapped_EthereumAddressAnyFormat_ConvertToChecksum(string ethereumAddress)
    {
        // Arrange
        // Act
        var wrappedToken = new TokenWrapped(5, new Address("PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT"),
            ExternalChainType.Ethereum, ethereumAddress, 5000);

        // Assert
        wrappedToken.NativeAddress.Should().Be("0x514910771AF9Ca656af840dff83E8264EcF986CA");
    }

    [Fact]
    public void TokenWrapped_BaseToken_ValidateTrue()
    {
        // Arrange
        // Act
        var wrappedToken = new TokenWrapped(5, new Address("PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT"),
                                            ExternalChainType.Ethereum, null, 5000);

        // Assert
        wrappedToken.Validated.Should().Be(true);
    }
}
