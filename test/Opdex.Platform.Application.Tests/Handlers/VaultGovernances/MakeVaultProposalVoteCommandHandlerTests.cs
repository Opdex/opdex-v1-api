using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Handlers.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultProposalVoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultProposalVoteCommandHandler _handler;

    public MakeVaultProposalVoteCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultProposalVoteCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_Refresh_RetrieveVaultGovernanceByIdQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, blockHeight);
        var request = new MakeVaultProposalVoteCommand(vote, blockHeight, true);

        // Act
        try
        {
            await _handler.Handle(request, CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == vote.VaultGovernanceId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Refresh_RetrieveVaultProposalByIdQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, blockHeight);
        var request = new MakeVaultProposalVoteCommand(vote, blockHeight, true);
        var expectedVault = new VaultGovernance(1, "PmH1iT1GLsMroh6zXXNMU9EjmivLgqqARw", 2, 3, 4, 5, 6, 7, 8, 9);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedVault);

        // Act
        try
        {
            await _handler.Handle(request, CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == vote.ProposalId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Refresh_CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, blockHeight);
        var request = new MakeVaultProposalVoteCommand(vote, blockHeight, true);
        var expectedVault = new VaultGovernance(1, "PmH1iT1GLsMroh6zXXNMU9EjmivLgqqARw", 2, 3, 4, 5, 6, 7, 8, 9);
        var expectedProposal = new VaultProposal(1, vote.ProposalId, 1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN",
                                                 2, "Description", VaultProposalType.Create, VaultProposalStatus.Vote, 6, 7, 8, 9, false, 10, 11);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVault);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProposal);

        // Act
        try
        {
            await _handler.Handle(request, CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery>(q => q.Vault == expectedVault.Address &&
                                                                                                                          q.ProposalId == vote.ProposalId &&
                                                                                                                          q.Voter == vote.Voter &&
                                                                                                                          q.BlockHeight == blockHeight), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        const ulong blockHeight = 50;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, blockHeight);
        var request = new MakeVaultProposalVoteCommand(vote, blockHeight);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultProposalVoteCommand>(command => command.Vote == vote), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        const ulong blockHeight = 50;
        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, blockHeight);
        var request = new MakeVaultProposalVoteCommand(vote, blockHeight);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultProposalVoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
