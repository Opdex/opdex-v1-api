using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Handlers.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances.Pledges;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultProposalPledgeCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultProposalPledgeCommandHandler _handler;

    public MakeVaultProposalPledgeCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultProposalPledgeCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_Refresh_RetrieveVaultGovernanceByIdQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, blockHeight);
        var request = new MakeVaultProposalPledgeCommand(pledge, blockHeight, true);

        // Act
        try
        {
            await _handler.Handle(request, CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == pledge.VaultId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Refresh_RetrieveVaultProposalByIdQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, blockHeight);
        var request = new MakeVaultProposalPledgeCommand(pledge, blockHeight, true);
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
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByIdQuery>(q => q.ProposalId == pledge.ProposalId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Refresh_CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery()
    {
        // Arrange
        const ulong blockHeight = 50;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, blockHeight);
        var request = new MakeVaultProposalPledgeCommand(pledge, blockHeight, true);
        var expectedVault = new VaultGovernance(1, "PmH1iT1GLsMroh6zXXNMU9EjmivLgqqARw", 2, 3, 4, 5, 6, 7, 8, 9);
        var expectedProposal = new VaultProposal(1, pledge.ProposalId, 1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN",
                                                 2, "Description", VaultProposalType.Create, VaultProposalStatus.Vote, 6, 7, 8, 9, true, 10, 11);

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
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery>(q => q.ProposalId == pledge.ProposalId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HappyPath_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        const ulong blockHeight = 50;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultProposalPledgeCommand(pledge, blockHeight);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultProposalPledgeCommand>(command => command.Pledge == pledge), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        const ulong blockHeight = 50;
        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var request = new MakeVaultProposalPledgeCommand(pledge, blockHeight);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultProposalPledgeCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
