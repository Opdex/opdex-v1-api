using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults;

public class VaultProposalVoteTests
{
    [Fact]
    public void CreateNewVaultProposalVote_InvalidVaultId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 0;
        const ulong proposalId = 2;
        Address voter = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong vote = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const bool inFavor = true;

        //Act
        void Act() => new VaultProposalVote(vaultId, proposalId, voter, vote, balance, inFavor, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("VaultId must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalVote_InvalidProposalId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 0;
        Address voter = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong vote = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const bool inFavor = true;

        //Act
        void Act() => new VaultProposalVote(vaultId, proposalId, voter, vote, balance, inFavor, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("ProposalId must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalVote_InvalidVoter_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address voter = "";
        const ulong vote = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const bool inFavor = true;

        //Act
        void Act() => new VaultProposalVote(vaultId, proposalId, voter, vote, balance, inFavor, createdBlock);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Voter must be provided.");
    }

    [Fact]
    public void CreateNewVaultProposalVote_InvalidVote_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address voter = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong vote = 0;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const bool inFavor = true;

        //Act
        void Act() => new VaultProposalVote(vaultId, proposalId, voter, vote, balance, inFavor, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Vote must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalVote_Success()
    {
        // Arrange
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address voter = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong vote = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const bool inFavor = true;

        //Act
        var proposalVote = new VaultProposalVote(vaultId, proposalId, voter, vote, balance, inFavor, createdBlock);

        // Assert
        proposalVote.Id.Should().Be(0ul);
        proposalVote.VaultId.Should().Be(vaultId);
        proposalVote.ProposalId.Should().Be(proposalId);
        proposalVote.Voter.Should().Be(voter);
        proposalVote.Vote.Should().Be(vote);
        proposalVote.Balance.Should().Be(balance);
        proposalVote.InFavor.Should().Be(inFavor);
        proposalVote.CreatedBlock.Should().Be(createdBlock);
        proposalVote.ModifiedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void CreateVaultProposalVote_Success()
    {
        // Arrange
        const ulong id = 99;
        const ulong vaultId = 1;
        const ulong proposalId = 2;
        Address voter = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong vote = 3;
        const ulong balance = 4;
        const ulong createdBlock = 5;
        const ulong modifiedBlock = 6;
        const bool inFavor = true;

        //Act
        var proposalVote = new VaultProposalVote(id, vaultId, proposalId, voter, vote, balance, inFavor, createdBlock, modifiedBlock);

        // Assert
        proposalVote.Id.Should().Be(id);
        proposalVote.VaultId.Should().Be(vaultId);
        proposalVote.ProposalId.Should().Be(proposalId);
        proposalVote.Voter.Should().Be(voter);
        proposalVote.Vote.Should().Be(vote);
        proposalVote.Balance.Should().Be(balance);
        proposalVote.InFavor.Should().Be(inFavor);
        proposalVote.CreatedBlock.Should().Be(createdBlock);
        proposalVote.ModifiedBlock.Should().Be(modifiedBlock);
    }
}
