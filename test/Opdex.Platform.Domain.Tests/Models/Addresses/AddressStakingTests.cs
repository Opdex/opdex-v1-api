using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses
{
    public class AddressStakingTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Constructor_LiquidityPoolIdNotValid_ThrowArgumentOutOfRangeException(long liquidityPoolId)
        {
            // Arrange
            // Act
            void Act() => new AddressStaking(liquidityPoolId, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity pool id must be greater than 0.");
        }

        [Fact]
        public void Constructor_OwnerNotValid_ThrowArgumentNullException()
        {
            // Arrange
            var owner = Address.Empty;

            // Act
            void Act() => new AddressStaking(1, owner, 9999999999999, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
        }

        [Fact]
        public void Constructor_ArgumentsValid_SetProperties()
        {
            // Arrange
            var liqudityPoolId = 1;
            Address owner = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            UInt256 weight = 9999999999999;
            ulong createdBlock = 100_000;

            // Act
            var addressStaking = new AddressStaking(liqudityPoolId, owner, weight, createdBlock);

            // Assert
            addressStaking.LiquidityPoolId.Should().Be(liqudityPoolId);
            addressStaking.Owner.Should().Be(owner);
            addressStaking.Weight.Should().Be(weight);
            addressStaking.CreatedBlock.Should().Be(createdBlock);
            addressStaking.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void SetWeight_LowerModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

            // Act
            void Act() => addressStaking.SetWeight(50000, 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Modified block cannot be before created block.");
        }

        [Fact]
        public void SetWeight_ValidArguments_SetProperties()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

            UInt256 updatedWeight = 50000;
            ulong updatedBlock = 100_001;

            // Act
            addressStaking.SetWeight(updatedWeight, updatedBlock);

            // Assert
            addressStaking.Weight.Should().Be(updatedWeight);
            addressStaking.ModifiedBlock.Should().Be(updatedBlock);
        }
    }
}
