using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets
{
    public class MarketPermissionTests
    {
        [Fact]
        public void CreateMarketPermission_InvalidUserAddress_ThrowArgumentNullException()
        {
            // Arrange
            var user = Address.Empty;

            // Act
            void Act() => new MarketPermission(5, user, Permissions.Trade, true, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 500);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("User address must be set.");
        }

        [Fact]
        public void CreateMarketPermission_UnknownPermission_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new MarketPermission(5,
                                                      "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                      0,
                                                      true,
                                                      "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                      500);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Permission must be valid.");
        }

        [Fact]
        public void CreateMarketPermission_InvalidBlameAddress_ThrowArgumentNullException()
        {
            // Arrange
            var blame = Address.Empty;

            // Act
            void Act() => new MarketPermission(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", Permissions.Trade, true, blame, 500);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Blame address must be set.");
        }

        [Fact]
        public void CreateMarketPermission_ValidArguments_SetProperties()
        {
            // Arrange
            var marketId = 5L;
            var user = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var permission = Permissions.Trade;
            var isAuthorized = true;
            var blame = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var createdBlock = 500UL;

            // Act
            var marketPermission = new MarketPermission(marketId, user, permission, isAuthorized, blame, createdBlock);

            // Assert
            marketPermission.MarketId.Should().Be(marketId);
            marketPermission.User.Should().Be(user);
            marketPermission.Permission.Should().Be(permission);
            marketPermission.IsAuthorized.Should().Be(isAuthorized);
            marketPermission.Blame.Should().Be(blame);
            marketPermission.CreatedBlock.Should().Be(createdBlock);
            marketPermission.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void Authorize_InvalidBlameAddress_ThrowArgumentNullException()
        {
            var blame = Address.Empty;

            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        Permissions.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            // Act
            void Act() => marketPermission.Authorize(blame, marketPermission.CreatedBlock + 100);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Blame address must be set.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Authorize_AnyState_UpdateBlameModifiedBlockAndAuthorized(bool alreadyAuthorized)
        {
            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        Permissions.Trade,
                                                        alreadyAuthorized,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            var blame = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXS";
            var modifiedBlock = marketPermission.CreatedBlock + 100;

            // Act
            marketPermission.Authorize(blame, modifiedBlock);

            // Assert
            marketPermission.IsAuthorized.Should().Be(true);
            marketPermission.Blame.Should().Be(blame);
            marketPermission.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void Revoke_InvalidBlameAddress_ThrowArgumentNullException()
        {
            var blame = Address.Empty;

            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        Permissions.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            // Act
            void Act() => marketPermission.Revoke(blame, marketPermission.CreatedBlock + 100);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Blame address must be set.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Revoke_AnyState_UpdateBlameModifiedBlockAndAuthorized(bool alreadyAuthorized)
        {
            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        Permissions.Trade,
                                                        alreadyAuthorized,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            var blame = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXS";
            var modifiedBlock = marketPermission.CreatedBlock + 100;

            // Act
            marketPermission.Revoke(blame, modifiedBlock);

            // Assert
            marketPermission.IsAuthorized.Should().Be(false);
            marketPermission.Blame.Should().Be(blame);
            marketPermission.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
