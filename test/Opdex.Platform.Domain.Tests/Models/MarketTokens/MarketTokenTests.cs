using FluentAssertions;
using Opdex.Platform.Domain.Models.MarketTokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MarketTokens
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

            // Act
            var marketToken = new MarketToken(id, marketId, tokenId, createdBlock, modifiedBlock);

            // Assert
            marketToken.Id.Should().Be(id);
            marketToken.MarketId.Should().Be(marketId);
            marketToken.TokenId.Should().Be(tokenId);
            marketToken.CreatedBlock.Should().Be(createdBlock);
            marketToken.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
