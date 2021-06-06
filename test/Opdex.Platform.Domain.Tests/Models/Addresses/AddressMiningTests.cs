using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses
{
    public class AddressMiningTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Constructor_MiningPoolIdNotValid_ThrowArgumentOutOfRangeException(long miningPoolId)
        {
            // Arrange
            // Act
            void Act() => new AddressMining(miningPoolId, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException(string owner)
        {
            // Arrange
            // Act
            void Act() => new AddressMining(1, owner, "9999999999999", 100_000);

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
            void Act() => new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", balance, 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void Constructor_ArgumentsValid_SetProperties()
        {
            // Arrange
            var miningPoolId = 1;
            var owner = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            var balance = "9999999999999";
            ulong createdBlock = 100_000;

            // Act
            var addressMining = new AddressMining(miningPoolId, owner, balance, createdBlock);

            // Assert
            addressMining.MiningPoolId.Should().Be(miningPoolId);
            addressMining.Owner.Should().Be(owner);
            addressMining.Balance.Should().Be(balance);
            addressMining.CreatedBlock.Should().Be(createdBlock);
            addressMining.ModifiedBlock.Should().Be(createdBlock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void SetBalance_BalanceNotValid_ThrowArgumentOutOfRangeException(string balance)
        {
            // Arrange
            var addressMining = new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            // Act
            void Act() => addressMining.SetBalance(balance, 100_001);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetBalance_LowerModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var addressMining = new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            // Act
            void Act() => addressMining.SetBalance("5000", 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetBalance_ArgumentsValid_PropertiesUpdated()
        {
            // Arrange
            var addressMining = new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            var updatedBalance = "5000";
            ulong updatedBlock = 100_001;

            // Act
            addressMining.SetBalance("5000", updatedBlock);

            // Assert
            addressMining.Balance.Should().Be(updatedBalance);
            addressMining.ModifiedBlock.Should().Be(updatedBlock);
        }
    }
}