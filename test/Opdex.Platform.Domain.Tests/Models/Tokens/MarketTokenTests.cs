using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens
{
    public class MarketTokenTests
    {
        [Fact]
        public void CreatesNew_MarketToken_InvalidMarketId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new MarketToken(0, 2, 3);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Market id must be greater than zero.");
        }

        [Fact]
        public void CreatesNew_MarketToken_InvalidTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new MarketToken(1, 0, 3);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token id must be greater than zero.");
        }

        [Fact]
        public void CreatesNew_MarketToken()
        {
            // Arrange
            const ulong marketId = 2;
            const ulong tokenId = 3;
            const ulong createdBlock = 10;

            // Act
            var marketToken = new MarketToken(marketId, tokenId, createdBlock);

            // Assert
            marketToken.Id.Should().Be(0ul);
            marketToken.MarketId.Should().Be(marketId);
            marketToken.TokenId.Should().Be(tokenId);
            marketToken.CreatedBlock.Should().Be(createdBlock);
            marketToken.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void CreatesExisting_MarketToken()
        {
            // Arrange
            const ulong id = 1;
            const ulong marketId = 2;
            const ulong tokenId = 3;
            const ulong createdBlock = 10;
            const ulong modifiedBlock = 11;
            Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            const string name = "Opdex Token";
            const bool isLpt = true;
            const string symbol = "OPDX";
            const int decimals = 18;
            const ulong sats = 10000000000000000;
            UInt256 totalSupply = 987654321;

            // Act
            var marketToken = new MarketToken(id, marketId, tokenId, address, isLpt, name, symbol, decimals, sats,
                                              totalSupply, createdBlock, modifiedBlock);

            // Assert
            marketToken.Id.Should().Be(id);
            marketToken.MarketId.Should().Be(marketId);
            marketToken.TokenId.Should().Be(tokenId);
            marketToken.Address.Should().Be(address);
            marketToken.IsLpt.Should().Be(isLpt);
            marketToken.Name.Should().Be(name);
            marketToken.Symbol.Should().Be(symbol);
            marketToken.Decimals.Should().Be(decimals);
            marketToken.Sats.Should().Be(sats);
            marketToken.TotalSupply.Should().Be(totalSupply);
            marketToken.CreatedBlock.Should().Be(createdBlock);
            marketToken.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
