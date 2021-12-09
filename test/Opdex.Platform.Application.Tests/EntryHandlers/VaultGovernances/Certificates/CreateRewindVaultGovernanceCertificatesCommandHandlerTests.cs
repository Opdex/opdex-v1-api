using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Certificates;

public class CreateRewindVaultGovernanceCertificatesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultGovernanceCertificatesCommandHandler _handler;

    public CreateRewindVaultGovernanceCertificatesCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultGovernanceCertificatesCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultGovernanceCertificatesCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultGovernanceCertificatesCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultGovernanceCertificatesCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultGovernanceCertificatesCommand_Sends_RetrieveVaultGovernanceCertificatesByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultGovernanceCertificatesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceCertificatesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultGovernanceCertificatesCommand_Sends_RetrieveVaultGovernanceByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 1;

        var certificates = new List<VaultCertificate> { new(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, true, 5, 6) };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificates);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultGovernanceCertificatesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == vaultId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultGovernanceCertificatesCommand_Sends_RetrieveVaultGovernanceContractCertificateSummaryByOwnerQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 1;
        var vault = new VaultGovernance(vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, 3, 4, 5, 6, 7, 8, 9);

        var certificates = new List<VaultCertificate> { new(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, true, 5, 10), new(2, vaultId, "PzHqqART1GLsMrwmoh61iU9EjmXXNMivLg", 4, 5, true, true, 6, 10) };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificates);

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == vaultId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vault);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultGovernanceCertificatesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var certificate in certificates)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceContractCertificateSummaryByOwnerQuery>(q => q.Vault == vault.Address &&
                                                                                                                             q.Owner == certificate.Owner &&
                                                                                                                             q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    // Difficult test to write due to grouping of vaults then owners along with summary updates.
    // Single vault, 2 owners, 1 cert each. Multiple certificates per owner will break the test.
    // See comment below for more details.
    [Fact]
    public async Task CreateRewindVaultGovernanceCertificatesCommand_Sends_MakeVaultGovernanceCertificateCommand()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong vaultId = 1;
        var vault = new VaultGovernance(vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, 3, 4, 5, 6, 7, 8, 9);

        var certificates = new List<VaultCertificate> {
            new(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, false, 5, 10),
            new(2, vaultId, "PzHqqART1GLsMrwmoh61iU9EjmXXNMivLg", 4, 5, true, true, 6, 10)
        };

        var summaries = new List<VaultContractCertificateSummary> { new(3, 4, false), new(2, 5, false) };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificates);

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByIdQuery>(q => q.VaultId == vaultId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vault);

        foreach (var certificate in certificates)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceContractCertificateSummaryByOwnerQuery>(q => q.Vault == vault.Address &&
                                                                                                                            q.Owner == certificate.Owner),
                                                  It.IsAny<CancellationToken>()))
                // Hack, VaultContractCertificateSummary doesn't reference an Owner because in code, its not needed.
                // For this, make each certificate a different vested block and use it as an Id. Limits us to 1 cert per owner in this test.
                .ReturnsAsync(summaries.First(summary => summary.VestedBlock == certificate.VestedBlock));
        }

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultGovernanceCertificatesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var certificate in certificates)
        {
            var summary = summaries.First(summary => summary.VestedBlock == certificate.VestedBlock);

            certificate.Update(summary, rewindHeight);

            _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultGovernanceCertificateCommand>(q => q.Certificate == certificate),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
