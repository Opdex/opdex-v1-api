using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Handlers.VaultGovernances;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.VaultGovernances;

public class MakeVaultGovernanceCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeVaultGovernanceCommandHandler _handler;

    public MakeVaultGovernanceCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeVaultGovernanceCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_RefreshFalse_DoNotRefresh()
    {
        // Arrange
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceContractSummaryQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RefreshTrue_Refresh()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000, refreshUnassignedSupply: true, refreshProposedSupply: true);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new VaultGovernanceContractSummary(request.BlockHeight));

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceContractSummaryQuery>(query => query.VaultGovernance == request.Vault.Address
                                                                                                        && query.BlockHeight == request.BlockHeight
                                                                                                        && query.IncludeUnassignedSupply == request.RefreshUnassignedSupply
                                                                                                        && query.IncludeProposedSupply == request.RefreshProposedSupply
                                                                                                  ), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RefreshFalse_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000);

        var originalUnassignedSupply = vault.UnassignedSupply;
        var originalProposedSupply = vault.ProposedSupply;

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultGovernanceCommand>(command => command.Vault == vault
                                                                                            && command.Vault.UnassignedSupply == originalUnassignedSupply
                                                                                            && command.Vault.ProposedSupply == originalProposedSupply
                                                                                    ), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_RefreshTrue_Persist()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000, refreshUnassignedSupply: true, refreshProposedSupply: true);

        var contractSummary = new VaultGovernanceContractSummary(request.BlockHeight);
        contractSummary.SetUnassignedSupply(new SmartContractMethodParameter((UInt256)500000000000000));
        contractSummary.SetProposedSupply(new SmartContractMethodParameter((UInt256)750000000000));

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(contractSummary);

        // Act
        await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultGovernanceCommand>(command => command.Vault == vault
                                                                                            && command.Vault.UnassignedSupply == contractSummary.UnassignedSupply.Value
                                                                                            && command.Vault.ProposedSupply == contractSummary.ProposedSupply.Value
                                                                                    ), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_OncePersisted_ReturnResult()
    {
        // Arrange
        var vault = new VaultGovernance("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 100000, 50000000, 1000000000, 50);
        var request = new MakeVaultGovernanceCommand(vault, 500000);

        ulong result = 25;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistVaultGovernanceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(result);
    }
}
