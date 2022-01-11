using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults;

public class VaultTests
{
    [Fact]
    public void CreateNewVault_InvalidAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address address = "";
        const ulong tokenId = 1;
        const ulong vestingDuration = 2;
        const ulong totalPledgeMinimum = 3;
        const ulong totalVoteMinimum = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new Vault(address, tokenId, vestingDuration, totalPledgeMinimum, totalVoteMinimum, createdBlock);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Address must be set.");
    }

    [Fact]
    public void CreateNewVault_InvalidTokenId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong tokenId = 0;
        const ulong vestingDuration = 2;
        const ulong totalPledgeMinimum = 3;
        const ulong totalVoteMinimum = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new Vault(address, tokenId, vestingDuration, totalPledgeMinimum, totalVoteMinimum, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("TokenId must be greater than zero.");
    }

    [Fact]
    public void CreateNewVault_InvalidVestingDuration_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong tokenId = 1;
        const ulong vestingDuration = 0;
        const ulong totalPledgeMinimum = 3;
        const ulong totalVoteMinimum = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new Vault(address, tokenId, vestingDuration, totalPledgeMinimum, totalVoteMinimum, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Vesting duration must be greater than zero.");
    }

    [Fact]
    public void CreateNewVault_Success()
    {
        // Arrange
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong tokenId = 1;
        const ulong vestingDuration = 2;
        const ulong totalPledgeMinimum = 3;
        const ulong totalVoteMinimum = 4;
        const ulong createdBlock = 5;

        //Act
        var vault = new Vault(address, tokenId, vestingDuration, totalPledgeMinimum, totalVoteMinimum, createdBlock);

        // Assert
        vault.Id.Should().Be(0ul);
        vault.Address.Should().Be(address);
        vault.TokenId.Should().Be(tokenId);
        vault.VestingDuration.Should().Be(vestingDuration);
        vault.UnassignedSupply.Should().Be(UInt256.Zero);
        vault.ProposedSupply.Should().Be(UInt256.Zero);
        vault.TotalPledgeMinimum.Should().Be(totalPledgeMinimum);
        vault.TotalVoteMinimum.Should().Be(totalVoteMinimum);
        vault.CreatedBlock.Should().Be(createdBlock);
        vault.ModifiedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void CreateVault_Success()
    {
        // Arrange
        const ulong id = 99;
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong tokenId = 1;
        const ulong vestingDuration = 2;
        const ulong totalPledgeMinimum = 3;
        const ulong totalVoteMinimum = 4;
        const ulong createdBlock = 5;
        const ulong modifiedBlock = 6;
        UInt256 unassignedSupply = 100;
        UInt256 proposedSupply = 200;

        //Act
        var vault = new Vault(id, address, tokenId, unassignedSupply, vestingDuration, proposedSupply, totalPledgeMinimum, totalVoteMinimum,
                              createdBlock, modifiedBlock);

        // Assert
        vault.Id.Should().Be(id);
        vault.Address.Should().Be(address);
        vault.TokenId.Should().Be(tokenId);
        vault.VestingDuration.Should().Be(vestingDuration);
        vault.UnassignedSupply.Should().Be(unassignedSupply);
        vault.ProposedSupply.Should().Be(proposedSupply);
        vault.TotalPledgeMinimum.Should().Be(totalPledgeMinimum);
        vault.TotalVoteMinimum.Should().Be(totalVoteMinimum);
        vault.CreatedBlock.Should().Be(createdBlock);
        vault.ModifiedBlock.Should().Be(modifiedBlock);
    }
}
