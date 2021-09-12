using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
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
            static void Act() => new AddressAllowance(0, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", new UInt256("50000000"), 10_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token id must be greater than 0.");
        }

        [Fact]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException()
        {
            // Arrange
            var owner = Address.Empty;

            // Act
            void Act() => new AddressAllowance(1, owner, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", new UInt256("50000000"), 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
        }

        [Fact]
        public void Constructor_SpenderNotValid_ThrowArgumentNullException()
        {
            // Arrange
            var spender = Address.Empty;

            // Act
            void Act() => new AddressAllowance(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", spender, new UInt256("50000000"), 10_001);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Spender must be set.");
        }

        [Fact]
        public void Constructor_ValidArguments_PropertiesSet()
        {
            // Arrange
            var tokenId = 102;
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address spender = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var allowance = new UInt256("50000000");
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

        [Fact]
        public void SetAllowance_ValidAmount_UpdateAllowance()
        {
            // Arrange
            var originalAmount = new UInt256("8888888888");
            var updatedAmount = new UInt256("50000000");
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
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", new UInt256("5555555"), 10_000,
                                                        10_500);

            // Act
            addressAllowance.SetAllowance(new UInt256("5555"), modifiedBlock);

            // Assert
            addressAllowance.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
