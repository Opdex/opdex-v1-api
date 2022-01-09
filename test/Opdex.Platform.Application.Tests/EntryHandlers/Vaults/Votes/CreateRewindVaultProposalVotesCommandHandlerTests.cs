using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Application.EntryHandlers.Vaults.Votes;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults.Votes;

public class CreateRewindVaultProposalVotesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultProposalVotesCommandHandler _handler;

    public CreateRewindVaultProposalVotesCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultProposalVotesCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultProposalVotesCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultProposalVotesCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultProposalVotesCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultProposalVotesCommand_Sends_RetrieveVaultProposalVotesByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalVotesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalVotesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalVotesCommand_Sends_RetrieveVaultByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 1;

        var votes = new List<VaultProposalVote>
        {
            new (1, vaultId, 1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, true, 6, rewindHeight),
            new (2, vaultId, 2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, true, 6, rewindHeight),
            new (3, vaultId, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, false, 6, rewindHeight),
            new (4, vaultId, 4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, false, 6, rewindHeight),
        };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(votes);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalVotesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByIdQuery>(q => q.VaultId == vaultId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalVotesCommand_Sends_RetrieveVaultProposalByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong proposalId = 2;

        var votes = new List<VaultProposalVote>
        {
            new (1, vaultId, proposalId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, true, 6, rewindHeight),
            new (2, vaultId, proposalId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, true, 6, rewindHeight),
            new (3, vaultId, proposalId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, false, 6, rewindHeight),
            new (4, vaultId, proposalId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, false, 6, rewindHeight),
        };

        var vault = new Vault(3, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalVotesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposalId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalVotesCommand_Sends_SelectTransactionForVaultProposalVoteRewindQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong publicId = 4;

        var votes = new List<VaultProposalVote>
        {
            new (1, vaultId, 1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, true, 6, rewindHeight),
            new (2, vaultId, 2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, true, 6, rewindHeight),
            new (3, vaultId, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, false, 6, rewindHeight),
            new (4, vaultId, 4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, false, 6, rewindHeight),
        };

        var proposals = new List<VaultProposal>
        {
            new (1, publicId, vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, "Description", VaultProposalType.Create, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (2, publicId, vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, "Description", VaultProposalType.Revoke, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (3, publicId, vaultId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Complete, 5, 6, 7, 8, true, 9, 12),
            new (4, publicId, vaultId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12)
        };

        var vault = new Vault(vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        foreach (var proposal in proposals)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposal.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposal);
        }

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalVotesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var vote in votes)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectTransactionForVaultProposalVoteRewindQuery>(q => q.Vault == vault.Address &&
                                                                                                                  q.Voter == vote.Voter &&
                                                                                                                  q.ProposalPublicId == publicId),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task CreateRewindVaultProposalVotesCommand_Sends_MakeVaultProposalVoteCommand()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong publicId = 4;

        var votes = new List<VaultProposalVote>
        {
            new (1, vaultId, 1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, true, 6, rewindHeight),
            new (2, vaultId, 2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, true, 6, rewindHeight),
            new (3, vaultId, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, false, 6, rewindHeight),
            new (4, vaultId, 4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, false, 6, rewindHeight),
        };

        var proposals = new List<VaultProposal>
        {
            new (1, publicId, vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, "Description", VaultProposalType.Create, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (2, publicId, vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, "Description", VaultProposalType.Revoke, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (3, publicId, vaultId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Complete, 5, 6, 7, 8, true, 9, 12),
            new (4, publicId, vaultId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12)
        };

        var vault = new Vault(vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        foreach (var proposal in proposals)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposal.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposal);
        }

        foreach (var vote in votes)
        {
            var transaction = new Transaction(1, new Sha256(5340958239), 2, 3, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null);
            var log = JsonConvert.SerializeObject(new VoteLogDetails
            {
                ProposalId = publicId,
                Voter = vote.Voter,
                VoteAmount = vote.Balance,
                VoterAmount = vote.Balance,
                ProposalYesAmount = 100,
                ProposalNoAmount = 100,
                InFavor = true
            });
            transaction.SetLog(new VaultProposalVoteLog(1, 1, vault.Address, 0, log));
            _mediator.Setup(callTo => callTo.Send(It.Is<SelectTransactionForVaultProposalVoteRewindQuery>(q => q.Vault == vault.Address &&
                                                                                                                 q.Voter == vote.Voter &&
                                                                                                                 q.ProposalPublicId == publicId),
                                                  It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
        }

        // Act
        await _handler.Handle(new CreateRewindVaultProposalVotesCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var vote in votes)
        {
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeVaultProposalVoteCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        }
    }

    private struct VoteLogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Voter { get; set; }
        public ulong VoteAmount { get; set; }
        public ulong VoterAmount { get; set; }
        public ulong ProposalYesAmount { get; set; }
        public ulong ProposalNoAmount { get; set; }
        public bool InFavor { get; set; }
    }
}
