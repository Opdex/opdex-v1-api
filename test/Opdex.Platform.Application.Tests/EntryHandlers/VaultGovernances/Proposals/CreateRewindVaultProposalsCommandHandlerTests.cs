using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Proposals;

public class CreateRewindVaultProposalsCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultProposalsCommandHandler _handler;

    public CreateRewindVaultProposalsCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultProposalsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultProposalsCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultProposalsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultProposalsCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultProposalsCommand_Sends_MakeVaultProposalCommand()
    {
        // Arrange
        const ulong rewindHeight = 12;

        var proposals = new List<VaultProposal>
        {
            new (1, 2, 3, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 4, "Description", VaultProposalType.Create, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12),
            new (2, 3, 3, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 4, "Description", VaultProposalType.Revoke, VaultProposalStatus.Vote, 5, 6, 7, 8, false, 9, 12),
            new (3, 4, 3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "Description", VaultProposalType.TotalVoteMinimum, VaultProposalStatus.Complete, 5, 6, 7, 8, true, 9, 12),
            new (4, 5, 3, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 4, "Description", VaultProposalType.TotalPledgeMinimum, VaultProposalStatus.Pledge, 5, 6, 7, 8, false, 9, 12)
        };

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultProposalsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                              It.IsAny<CancellationToken>())).ReturnsAsync(proposals);

        // Act
        await _handler.Handle(new CreateRewindVaultProposalsCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var proposal in proposals)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultProposalCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                        q.Proposal == proposal &&
                                                                                        q.RefreshProposal == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
