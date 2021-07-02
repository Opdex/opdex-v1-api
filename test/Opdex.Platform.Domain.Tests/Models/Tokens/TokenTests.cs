using FluentAssertions;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens
{
    public class TokenTests
    {
        [Fact]
        public void CreatesNewToken_Success()
        {
            const string address = "Address";
            const string name = "Opdex Token";
            const bool isLpt = true;
            const string symbol = "OPDX";
            const int decimals = 18;
            const long sats = 10000000000000000;
            const string totalSupply = "987654321";
            const ulong createdBlock = 3;

            var token = new Token(address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock);

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
            static void Act() => new Token("", true, "name", "symbol", 8, 100_000_000, "100", 2);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token address must be set.");
        }

        [Fact]
        public static void Constructor_NameInvalid_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            static void Act() => new Token("address", true, "", "symbol", 8, 100_000_000, "100", 2);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token name must be set.");
        }

        [Fact]
        public static void Constructor_SymbolInvalid_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            static void Act() => new Token("address", true, "name", "", 8, 100_000_000, "100", 2);

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
            void Act() => new Token("address", true, "name", "symbol", decimals, 100_000_000, "100", 2);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token must have between 0 and 18 decimal denominations.");
        }

        [Fact]
        public static void Constructor_SatsInvalid_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new Token("address", true, "name", "symbol", 8, 0, "100", 2);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Sats must be greater than zero.");
        }

        [Fact]
        public static void Constructor_TotalSupplyInvalid_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new Token("address", true, "name", "symbol", 8, 100_000_000, "", 2);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Total supply must only contain numeric digits.");
        }

        [Fact]
        public void CreatesExistingToken_Success()
        {
            const long id = 1;
            const string address = "Address";
            const string name = "Opdex Token";
            const bool isLpt = true;
            const string symbol = "OPDX";
            const int decimals = 18;
            const long sats = 10000000000000000;
            const string totalSupply = "987654321";
            const ulong createdBlock = 3;
            const ulong modifiedBlock = 4;

            var token = new Token(id, address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock);

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
        public void Token_SetMarket_Success()
        {
            const long marketId = 10;

            var token = new Token("address", true, "name", "symbol", 8, 100_000_000, "100", 2);

            token.SetMarket(marketId);

            token.MarketId.Should().Be(marketId);
        }

        [Fact]
        public void Token_UpdateTotalSupply_Success()
        {
            const string totalSupply = "200";
            const ulong updateBlock = 10;

            var token = new Token("address", true, "name", "symbol", 8, 100_000_000, "100", 2);

            token.UpdateTotalSupply(totalSupply, updateBlock);

            token.TotalSupply.Should().Be(totalSupply);
            token.ModifiedBlock.Should().Be(updateBlock);
        }

        [Fact]
        public void Token_UpdateTotalSupply_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var token = new Token("address", true, "name", "symbol", 8, 100_000_000, "100", 2);

            // Act
            void Act() => token.UpdateTotalSupply("1.25", 10);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Total supply must be a numeric value.");
        }
    }
}
