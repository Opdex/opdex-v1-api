using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses
{
    public class AddressBalanceTests
    {
        [Fact]
        public void Constructor_TokenAndLiquidityPoolIdBothZero_ThrowException()
        {
            // Arrange
            // Act
            static void Act() => new AddressBalance(0, 0, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000", 10_001);

            // Assert
            Assert.Throws<Exception>(Act);
        }

        [Fact]
        public void Constructor_NeitherTokenOrLiquidityPoolIdZero_ThrowException()
        {
            // Arrange
            // Act
            static void Act() => new AddressBalance(1, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000", 10_001);

            // Assert
            Assert.Throws<Exception>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException(string owner)
        {
            // Arrange
            // Act
            void Act() => new AddressBalance(0, 1, owner, "50000", 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void Constructor_BalanceNotValid_ThrowArgumentOutOfRangeException(string balance)
        {
            // Arrange
            // Act
            void Act() => new AddressBalance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", balance, 10_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void Constructor_ValidArguments_PropertiesSet()
        {
            // Arrange
            var tokenId = 0;
            var liquidityPoolId = 102;
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var balance = "50000000";
            ulong createdBlock = 10_001;

            // Act
            var addressBalance = new AddressBalance(tokenId, liquidityPoolId, owner, balance, createdBlock);

            // Assert
            addressBalance.TokenId.Should().Be(tokenId);
            addressBalance.LiquidityPoolId.Should().Be(liquidityPoolId);
            addressBalance.Owner.Should().Be(owner);
            addressBalance.Balance.Should().Be(balance);
            addressBalance.CreatedBlock.Should().Be(createdBlock);
            addressBalance.ModifiedBlock.Should().Be(createdBlock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void SetBalance_InvalidBalance_ThrowArgumentOutOfRangeException(string balance)
        {
            // Arrange
            var addressBalance = new AddressBalance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_000);

            // Act
            void Act() => addressBalance.SetBalance(balance, 10_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetBalance_PreviousBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var addressBalance = new AddressBalance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_000);

            // Act
            void Act() => addressBalance.SetBalance("9000000000", 9_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetBalance_ValidArguments_PropertiesSet()
        {
            // Arrange
            var addressBalance = new AddressBalance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_000);

            var updatedBalance = "9000000000";
            ulong updatedBlock = 10_001;

            // Act
            addressBalance.SetBalance(updatedBalance, updatedBlock);

            // Assert
            addressBalance.Balance.Should().Be(updatedBalance);
            addressBalance.ModifiedBlock.Should().Be(updatedBlock);
        }
    }
}