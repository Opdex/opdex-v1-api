using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses
{
    public class AddressAllowanceTests
    {
        [Fact]
        public void Constructor_TokenIdZero_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new AddressAllowance(0, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token id must be greater than 0.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException(string owner)
        {
            // Arrange
            // Act
            void Act() => new AddressAllowance(1, owner, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_SpenderNotValid_ThrowArgumentNullException(string spender)
        {
            // Arrange
            // Act
            void Act() => new AddressAllowance(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", spender, "50000000", 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Spender must be set.");
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
            void Act() => new AddressAllowance(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", allowance, 10_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Allowance must only contain numeric digits.");
        }

        [Fact]
        public void Constructor_ValidArguments_PropertiesSet()
        {
            // Arrange
            var tokenId = 102;
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var spender = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var allowance = "50000000";
            ulong createdBlock = 10_001;

            // Act
            var addressAllowance = new AddressAllowance(tokenId, owner, spender, allowance, createdBlock);

            // Assert
            addressAllowance.TokenId.Should().Be(tokenId);
            addressAllowance.Owner.Should().Be(owner);
            addressAllowance.Spender.Should().Be(spender);
            addressAllowance.Allowance.Should().Be(allowance);
            addressAllowance.CreatedBlock.Should().Be(createdBlock);
            addressAllowance.ModifiedBlock.Should().Be(createdBlock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void SetAllowance_AmountNotValid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            var addressAllowance = new AddressAllowance(1, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "5000000000", 10_000,
                                                        10_500);

            // Act
            void Act() => addressAllowance.SetAllowance(amount, 10_505);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount must only contain numeric digits.");
        }

        [Fact]
        public void SetAllowance_ValidAmount_UpdateAllowance()
        {
            // Arrange
            var originalAmount = "8888888888";
            var updatedAmount = "50000000";
            var addressAllowance = new AddressAllowance(1, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", originalAmount, 10_000,
                                                        10_500);

            // Act
            addressAllowance.SetAllowance(updatedAmount, 10_505);

            // Assert
            addressAllowance.Allowance.Should().Be(updatedAmount);
        }

        [Fact]
        public void SetAllowance_ValidAmount_UpdateModifiedBlock()
        {
            // Arrange
            var modifiedBlock = 10_505UL;
            var addressAllowance = new AddressAllowance(1, 1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "5555555", 10_000,
                                                        10_500);

            // Act
            addressAllowance.SetAllowance("5555", modifiedBlock);

            // Assert
            addressAllowance.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
