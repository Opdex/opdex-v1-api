using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses;

public class AddressMiningTests
{
    [Theory]
    [InlineData(0)]
    public void Constructor_MiningPoolIdNotValid_ThrowArgumentOutOfRangeException(ulong miningPoolId)
    {
        // Arrange
        // Act
        void Act() => new AddressMining(miningPoolId, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining pool id must be greater than 0.");
    }

    [Fact]
    public void Constructor_OwnerNotValid_ThrowArgumentNullException()
    {
        // Arrange
        var owner = Address.Empty;

        // Act
        void Act() => new AddressMining(1, owner, 9999999999999, 100_000);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
    }

    [Fact]
    public void Constructor_ArgumentsValid_SetProperties()
    {
        // Arrange
        var miningPoolId = 1ul;
        Address owner = "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST";
        UInt256 balance = 9999999999999;
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

    [Fact]
    public void SetBalance_LowerModifiedBlock_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var addressMining = new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

        // Act
        void Act() => addressMining.SetBalance(5000, 99_999);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Modified block cannot be before created block.");
    }

    [Fact]
    public void SetBalance_ArgumentsValid_PropertiesUpdated()
    {
        // Arrange
        var addressMining = new AddressMining(1, "PXLFzhR6jaHa1oT6kiSdmgS1tH23X3XeST", 9999999999999, 100_000);

        UInt256 updatedBalance = 5000;
        ulong updatedBlock = 100_001;

        // Act
        addressMining.SetBalance(5000, updatedBlock);

        // Assert
        addressMining.Balance.Should().Be(updatedBalance);
        addressMining.ModifiedBlock.Should().Be(updatedBlock);
    }
}