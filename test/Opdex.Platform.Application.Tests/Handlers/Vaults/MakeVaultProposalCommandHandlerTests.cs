using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Handlers.Vaults.Proposals;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Vaults;

public class MakeVaultProposalCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultProposalCommandHandler _handler;

    public MakeVaultProposalCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultProposalCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_Refresh_GetVault()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        const ulong blockHeight = 50;

        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, blockHeight);
        var request = new MakeVaultProposalCommand(proposal, blockHeight, true);

        // Act
        try
        {
            await _handler.Handle(request, cancellationTokenSource.Token);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByIdQuery>(command => command.VaultId == proposal.VaultId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Refresh_CallCirrus()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        const ulong blockHeight = 50;
        Address vault = "PMU9EGLsMroh6zXXNjmivLgqqARwmH1iT1";

        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, blockHeight);
        var request = new MakeVaultProposalCommand(proposal, blockHeight, true);
        var governance = new Vault(proposal.VaultId, vault, 1, 2, 3, 4, 5, 6, 7, 8);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(governance);

        // Act
        try
        {
            await _handler.Handle(request, cancellationTokenSource.Token);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetVaultProposalSummaryByProposalIdQuery>(command => command.Vault == vault  &&
                                                                                                                    command.ProposalId == proposal.PublicId &&
                                                                                                                    command.BlockHeight == blockHeight), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        const ulong blockHeight = 50;

        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, blockHeight);
        var request = new MakeVaultProposalCommand(proposal, blockHeight);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultProposalCommand>(command => command.Proposal == proposal), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        const ulong blockHeight = 50;
        var proposal = new VaultProposal(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000, blockHeight);
        var request = new MakeVaultProposalCommand(proposal, blockHeight);

        const ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultProposalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
