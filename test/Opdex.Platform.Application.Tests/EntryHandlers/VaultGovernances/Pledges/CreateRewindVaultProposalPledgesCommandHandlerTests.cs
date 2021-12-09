using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Pledges;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Pledges;

public class CreateRewindVaultProposalPledgesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultProposalPledgesCommandHandler _handler;

    public CreateRewindVaultProposalPledgesCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultProposalPledgesCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultProposalPledgesCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultProposalPledgesCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultProposalPledgesCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultProposalPledgesCommand_Sends_RetrieveVaultProposalPledgesByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalPledgesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalPledgesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalPledgesCommand_Sends_RetrieveVaultGovernanceByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 1;

        var pledges = new List<VaultProposalPledge>
        {
            new (1, vaultId, 2, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, 6, rewindHeight),
            new (1, vaultId, 3, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, 6, rewindHeight),
            new (1, vaultId, 4, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, rewindHeight),
            new (1, vaultId, 5, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, 6, rewindHeight),
        };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pledges);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalPledgesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == vaultId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalPledgesCommand_Sends_RetrieveVaultProposalByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong proposalId = 2;

        var pledges = new List<VaultProposalPledge>
        {
            new (1, vaultId, proposalId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, 6, rewindHeight),
            new (1, vaultId, proposalId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, 6, rewindHeight),
            new (1, vaultId, proposalId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, rewindHeight),
            new (1, vaultId, proposalId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, 6, rewindHeight),
        };

        var vault = new VaultGovernance(3, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalPledgesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposalId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultProposalPledgesCommand_Sends_SelectTransactionForVaultProposalPledgeRewindQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong publicId = 4;

        var pledges = new List<VaultProposalPledge>
        {
            new (1, vaultId, 1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, 6, rewindHeight),
            new (2, vaultId, 2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, 6, rewindHeight),
            new (3, vaultId, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, rewindHeight),
            new (4, vaultId, 4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, 6, rewindHeight),
        };

        var proposals = new List<VaultProposal>
        {
            new (1, publicId, vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, "Description", VaultProposalType.Create, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12),
            new (2, publicId, vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, "Description", VaultProposalType.Revoke, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (3, publicId, vaultId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Complete, 5, 6, 7, 8, true, 9, 12),
            new (4, publicId, vaultId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, "Description", VaultProposalType.TotalPledgeMinimum, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12)
        };

        var vault = new VaultGovernance(vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        foreach (var proposal in proposals)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposal.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposal);
        }

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultProposalPledgesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var pledge in pledges)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectTransactionForVaultProposalPledgeRewindQuery>(q => q.Vault == vault.Address &&
                                                                                                                  q.Pledger == pledge.Pledger &&
                                                                                                                  q.ProposalPublicId == publicId),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task CreateRewindVaultProposalPledgesCommand_Sends_MakeVaultProposalPledgeCommand()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 3;
        const ulong publicId = 4;

        var pledges = new List<VaultProposalPledge>
        {
            new (1, vaultId, 1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, 5, 6, rewindHeight),
            new (2, vaultId, 2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, 5, 6, rewindHeight),
            new (3, vaultId, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, rewindHeight),
            new (4, vaultId, 4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, 5, 6, rewindHeight),
        };

        var proposals = new List<VaultProposal>
        {
            new (1, publicId, vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, "Description", VaultProposalType.Create, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12),
            new (2, publicId, vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, "Description", VaultProposalType.Revoke, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (3, publicId, vaultId, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Complete, 5, 6, 7, 8, true, 9, 12),
            new (4, publicId, vaultId, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, "Description", VaultProposalType.TotalPledgeMinimum, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12)
        };

        var vault = new VaultGovernance(vaultId, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 2, 4, 5, 6, 7, 8, 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesByModifiedBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        foreach (var proposal in proposals)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == proposal.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposal);
        }

        foreach (var pledge in pledges)
        {
            var transaction = new Transaction(1, new Sha256(5340958239), 2, 3, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null);
            var log = JsonConvert.SerializeObject(new PledgeLogDetails
            {
                ProposalId = publicId,
                Pledger = pledge.Pledger,
                PledgeAmount = pledge.Balance,
                PledgerAmount = pledge.Balance,
                ProposalPledgeAmount = 100,
                TotalPledgeMinimumMet = true
            });
            transaction.SetLog(new VaultProposalPledgeLog(1, 1, vault.Address, 0, log));
            _mediator.Setup(callTo => callTo.Send(It.Is<SelectTransactionForVaultProposalPledgeRewindQuery>(q => q.Vault == vault.Address &&
                                                                                                                 q.Pledger == pledge.Pledger &&
                                                                                                                 q.ProposalPublicId == publicId),
                                                  It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
        }

        // Act
        await _handler.Handle(new CreateRewindVaultProposalPledgesCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var pledge in pledges)
        {
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeVaultProposalPledgeCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        }
    }

    private struct PledgeLogDetails
    {
        public ulong ProposalId;
        public Address Pledger;
        public ulong PledgeAmount;
        public ulong PledgerAmount;
        public ulong ProposalPledgeAmount;
        public bool TotalPledgeMinimumMet;
    }
}
