using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults;

public class VaultProposalPledgeTests
{
    [Fact]
    public void CreateNewVaultProposalPledge_InvalidVaultId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 0;
        const ulong proposalId = 2;
        Address pledger = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong pledge = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new VaultProposalPledge(vaultId, proposalId, pledger, pledge, balance, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Vault id must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalPledge_InvalidProposalId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 0;
        Address pledger = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong pledge = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new VaultProposalPledge(vaultId, proposalId, pledger, pledge, balance, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("ProposalId must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalPledge_InvalidPledger_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address pledger = "";
        const ulong pledge = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new VaultProposalPledge(vaultId, proposalId, pledger, pledge, balance, createdBlock);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Pledger must be provided.");
    }

    [Fact]
    public void CreateNewVaultProposalPledge_InvalidPledge_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address pledger = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong pledge = 0;
        const ulong balance = 4;
        const ulong createdBlock = 5;

        //Act
        void Act() => new VaultProposalPledge(vaultId, proposalId, pledger, pledge, balance, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Pledge must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalPledge_Success()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address pledger = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong pledge = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;

        //Act
        var proposalPledge = new VaultProposalPledge(vaultId, proposalId, pledger, pledge, balance, createdBlock);

        // Assert
        proposalPledge.Id.Should().Be(0ul);
        proposalPledge.VaultId.Should().Be(vaultId);
        proposalPledge.ProposalId.Should().Be(proposalId);
        proposalPledge.Pledger.Should().Be(pledger);
        proposalPledge.Pledge.Should().Be(pledge);
        proposalPledge.Balance.Should().Be(balance);
        proposalPledge.CreatedBlock.Should().Be(createdBlock);
        proposalPledge.ModifiedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void CreateVaultProposalPledge_Success()
    {
        // Arrange
        const ulong id = 99;
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address pledger = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong pledge = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const ulong modifiedBlock = 6;

        //Act
        var proposalPledge = new VaultProposalPledge(id, vaultId, proposalId, pledger, pledge, balance, createdBlock, modifiedBlock);

        // Assert
        proposalPledge.Id.Should().Be(id);
        proposalPledge.VaultId.Should().Be(vaultId);
        proposalPledge.ProposalId.Should().Be(proposalId);
        proposalPledge.Pledger.Should().Be(pledger);
        proposalPledge.Pledge.Should().Be(pledge);
        proposalPledge.Balance.Should().Be(balance);
        proposalPledge.CreatedBlock.Should().Be(createdBlock);
        proposalPledge.ModifiedBlock.Should().Be(modifiedBlock);
    }
}
