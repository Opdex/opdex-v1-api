using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Addresses;

public class AddressBalanceTests
{
    [Fact]
    public void Constructor_TokenAndLiquidityPoolIdBothZero_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new AddressBalance(0, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 50000, 10_001);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token id must be greater than 0.");
    }

    [Fact]
    public void Constructor_OwnerNotValid_ThrowArgumentNullException()
    {
        // Arrange
        var owner = Address.Empty;

        // Act
        void Act() => new AddressBalance(1, owner, 50000, 10_001);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
    }

    [Fact]
    public void Constructor_ValidArguments_PropertiesSet()
    {
        // Arrange
        ulong tokenId = 123;
        Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        UInt256 balance = 50000000;
        ulong createdBlock = 10_001;

        // Act
        var addressBalance = new AddressBalance(tokenId, owner, balance, createdBlock);

        // Assert
        addressBalance.TokenId.Should().Be(tokenId);
        addressBalance.Owner.Should().Be(owner);
        addressBalance.Balance.Should().Be(balance);
        addressBalance.CreatedBlock.Should().Be(createdBlock);
        addressBalance.ModifiedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void SetBalance_PreviousBlock_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var addressBalance = new AddressBalance(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 50000000, 10_000);

        // Act
        void Act() => addressBalance.SetBalance(9000000000, 9_999);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Modified block cannot be before created block.");
    }

    [Fact]
    public void SetBalance_ValidArguments_PropertiesSet()
    {
        // Arrange
        var addressBalance = new AddressBalance(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 50000000, 10_000);

        UInt256 updatedBalance = 9000000000;
        ulong updatedBlock = 10_001;

        // Act
        addressBalance.SetBalance(updatedBalance, updatedBlock);

        // Assert
        addressBalance.Balance.Should().Be(updatedBalance);
        addressBalance.ModifiedBlock.Should().Be(updatedBlock);
    }
}