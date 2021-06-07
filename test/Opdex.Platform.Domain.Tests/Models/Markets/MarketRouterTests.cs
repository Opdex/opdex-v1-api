using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets
{
    public class MarketRouterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateMarketRouter_InvalidAddress_ThrowArgumentNullException(string address)
        {
            // Arrange
            // Act
            void Act() => new MarketRouter(address, 2, true, 1);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Router address must be set.");
        }

        [Fact]
        public void CreateMarketRouter_InvalidMarketId_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const long marketId = 0;

            // Act
            void Act() => new MarketRouter("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", marketId, true, 1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("MarketId must be greater than 0.");
        }

        [Fact]
        public void CreateMarketRouter_ValidArguments_PropertiesAreSet()
        {
            // Arrange
            var address = "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi";
            var marketId = 10;
            var isActive = true;
            ulong createdBlock = 100_000;

            // Act
            var market = new MarketRouter(address, marketId, isActive, createdBlock);

            // Assert
            market.Address.Should().Be(address);
            market.MarketId.Should().Be(marketId);
            market.IsActive.Should().Be(isActive);
            market.CreatedBlock.Should().Be(createdBlock);
            market.ModifiedBlock.Should().Be(createdBlock);
        }
    }
}