using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens;

public class TokenTests
{
    [Fact]
    public void CreatesNewToken_Success()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const string name = "Opdex Token";
        const string symbol = "OPDX";
        const int decimals = 18;
        const ulong sats = 10000000000000000;
        UInt256 totalSupply = 987654321;
        const ulong createdBlock = 3;

        var token = new Token(address, name, symbol, decimals, sats, totalSupply, createdBlock);

        token.Id.Should().Be(0);
        token.Address.Should().Be(address);
        token.Name.Should().Be(name);
        token.Symbol.Should().Be(symbol);
        token.Decimals.Should().Be(decimals);
        token.Sats.Should().Be(sats);
        token.TotalSupply.Should().Be(totalSupply);
    }

    [Fact]
    public static void Constructor_AddressInvalid_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        static void Act() => new Token(Address.Empty, "name", "symbol", 8, 100_000_000, 100, 2);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token address must be set.");
    }

    [Fact]
    public static void Constructor_NameInvalid_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        static void Act() => new Token("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "", "symbol", 8, 100_000_000, 100, 2);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token name must be set.");
    }

    [Fact]
    public static void Constructor_SymbolInvalid_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        static void Act() => new Token("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "name", "", 8, 100_000_000, 100, 2);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token symbol must be set.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(19)]
    public static void Constructor_DecimalsInvalid_ThrowArgumentOutOfRangeException(int decimals)
    {
        // Arrange
        // Act
        void Act() => new Token("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "name", "symbol", decimals, 100_000_000, 100, 2);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token must have between 0 and 18 decimal denominations.");
    }

    [Fact]
    public static void Constructor_SatsInvalid_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new Token("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "name", "symbol", 8, 0, 100, 2);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Sats must be greater than zero.");
    }

    [Fact]
    public void CreatesExistingToken_Success()
    {
        const ulong id = 1;
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const string name = "Opdex Token";
        const string symbol = "OPDX";
        const int decimals = 18;
        const ulong sats = 10000000000000000;
        UInt256 totalSupply = 987654321;
        const ulong createdBlock = 3;
        const ulong modifiedBlock = 4;
        TokenSummary summary = new TokenSummary(5, 10, 50);

        var token = new Token(id, address, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock);

        token.Id.Should().Be(id);
        token.Address.Should().Be(address);
        token.Name.Should().Be(name);
        token.Symbol.Should().Be(symbol);
        token.Decimals.Should().Be(decimals);
        token.Sats.Should().Be(sats);
        token.TotalSupply.Should().Be(totalSupply);
        token.CreatedBlock.Should().Be(createdBlock);
        token.ModifiedBlock.Should().Be(modifiedBlock);
    }

    [Fact]
    public void Token_UpdateTotalSupply_Success()
    {
        UInt256 totalSupply = 200;
        const ulong updateBlock = 10;

        var token = new Token("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "name", "symbol", 8, 100_000_000, 100, 2);

        token.UpdateTotalSupply(totalSupply, updateBlock);

        token.TotalSupply.Should().Be(totalSupply);
        token.ModifiedBlock.Should().Be(updateBlock);
    }
}
