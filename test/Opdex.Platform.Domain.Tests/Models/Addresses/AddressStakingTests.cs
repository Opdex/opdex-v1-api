using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
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
            void Act() => new AddressStaking(liquidityPoolId, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

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
            void Act() => new AddressStaking(1, owner, "9999999999999", 100_000);

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
        public void Constructor_WeightNotValid_ThrowArgumentOutOfRangeException(string weight)
        {
            // Arrange
            // Act
            void Act() => new AddressStaking(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", weight, 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void Constructor_ArgumentsValid_SetProperties()
        {
            // Arrange
            var liqudityPoolId = 1;
            var owner = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            var weight = "9999999999999";
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
            var addressStaking = new AddressStaking(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = "50000";
            log.totalStaked = "100000";

            // Act
            void Act() => addressStaking.SetWeight(new StartStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetWeight_ValidArguments_SetProperties()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "9999999999999", 100_000);

            var updatedWeight = "50000";
            ulong updatedBlock = 100_001;

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = updatedWeight;
            log.totalStaked = "100000";

            // Act
            addressStaking.SetWeight(new StartStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), updatedBlock);

            // Assert
            addressStaking.Weight.Should().Be(updatedWeight);
            addressStaking.ModifiedBlock.Should().Be(updatedBlock);
        }

        [Fact]
        public void ResetWeight_IdIsNotSet_ThrowInvalidOperationException()
        {
            // Arrange
            var addressStaking = new AddressStaking(0, 1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "5000", 100_000, 100_000);

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = "5000";
            log.totalStaked = "100000";

            // Act
            void Act() => addressStaking.ResetWeight(new StopStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), 100_001);

            // Assert
            Assert.Throws<InvalidOperationException>(Act);
        }

        [Fact]
        public void ResetWeight_WeightIsNotEqualToLogAmount_ThrowInvalidOperationException()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, 1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "5000", 100_000, 100_000);

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = "9999";
            log.totalStaked = "100000";

            // Act
            void Act() => addressStaking.ResetWeight(new StopStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), 100_001);

            // Assert
            Assert.Throws<InvalidOperationException>(Act);
        }

        [Fact]
        public void ResetWeight_LowerModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, 1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "5000", 100_000, 100_000);

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = "5000";
            log.totalStaked = "100000";

            // Act
            void Act() => addressStaking.ResetWeight(new StopStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void ResetWeight_ArgumentsValid_PropertiesSet()
        {
            // Arrange
            var addressStaking = new AddressStaking(1, 1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", "5000", 100_000, 100_000);

            ulong updatedBlock = 100_001;

            dynamic log = new ExpandoObject();
            log.staker = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
            log.amount = "5000";
            log.totalStaked = "100000";

            // Act
            addressStaking.ResetWeight(new StopStakingLog(log, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 0), updatedBlock);

            // Assert
            addressStaking.Weight.Should().Be("0");
            addressStaking.ModifiedBlock.Should().Be(updatedBlock);
        }
    }
}