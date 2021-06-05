using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses
{
    public class AddressAllowanceTests
    {
        [Fact]
        public void Constructor_TokenAndLiquidityPoolIdBothZero_ThrowArgumentException()
        {
            // Arrange
            // Act
            static void Act() => new AddressAllowance(0, 0, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentException>(Act);
        }

        [Fact]
        public void Constructor_NeitherTokenOrLiquidityPoolIdZero_ThrowArgumentException()
        {
            // Arrange
            // Act
            static void Act() => new AddressAllowance(1, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException(string owner)
        {
            // Arrange
            // Act
            void Act() => new AddressAllowance(0, 1, owner, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_SpenderNotValid_ThrowArgumentNullException(string spender)
        {
            // Arrange
            // Act
            void Act() => new AddressAllowance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", spender, "50000000", 10_001);

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
        public void Constructor_AllowanceNotValid_ThrowArgumentOutOfRangeException(string allowance)
        {
            // Arrange
            // Act
            void Act() => new AddressAllowance(0, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", allowance, 10_001);

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
            var spender = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var allowance = "50000000";
            ulong createdBlock = 10_001;

            // Act
            var addressAllowance = new AddressAllowance(tokenId, liquidityPoolId, owner, spender, allowance, createdBlock);

            // Assert
            addressAllowance.TokenId.Should().Be(tokenId);
            addressAllowance.LiquidityPoolId.Should().Be(liquidityPoolId);
            addressAllowance.Owner.Should().Be(owner);
            addressAllowance.Spender.Should().Be(spender);
            addressAllowance.Allowance.Should().Be(allowance);
            addressAllowance.CreatedBlock.Should().Be(createdBlock);
            addressAllowance.ModifiedBlock.Should().Be(createdBlock);
        }
    }
}
