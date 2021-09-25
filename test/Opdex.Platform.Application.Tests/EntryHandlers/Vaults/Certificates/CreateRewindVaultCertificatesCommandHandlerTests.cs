using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Application.EntryHandlers.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults.Certificates
{
    public class CreateRewindVaultCertificatesCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindVaultCertificatesCommandHandler _handler;

        public CreateRewindVaultCertificatesCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindVaultCertificatesCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultCertificatesCommandHandler>>());
        }

        [Fact]
        public void CreateRewindVaultCertificatesCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindVaultCertificatesCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindVaultCertificatesCommand_Sends_RetrieveVaultsByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindVaultCertificatesCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultCertificatesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindVaultCertificatesCommand_Sends_RetrieveVaultByIdQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;
            const long vaultId = 1;

            var certificates = new List<VaultCertificate>
            {
                new VaultCertificate(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, true, 5, 6)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(certificates);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindVaultCertificatesCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByIdQuery>(q => q.VaultId == vaultId),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindVaultCertificatesCommand_Sends_RetrieveVaultContractCertificateSummariesByOwnerQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;
            const long vaultId = 1;
            var vault = new Vault(vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, "PLsMroh6zwmH1iU9EjmXXNMivLgqqART1G", 3, 4, 5, 6);

            var certificates = new List<VaultCertificate>
            {
                new VaultCertificate(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, true, 5, 10),
                new VaultCertificate(2, vaultId, "PzHqqART1GLsMrwmoh61iU9EjmXXNMivLg", 4, 5, true, true, 6, 10)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(certificates);

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultByIdQuery>(q => q.VaultId == vaultId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vault);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindVaultCertificatesCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            foreach (var certificate in certificates)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultContractCertificateSummariesByOwnerQuery>(q => q.Vault == vault.Address &&
                                                                                                                         q.Owner == certificate.Owner &&
                                                                                                                         q.BlockHeight == rewindHeight),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        // Difficult test to write due to grouping of vaults then owners along with summary updates.
        // Single vault, 2 owners, 1 cert each. Multiple certificates per owner will break the test.
        // See comment below for more details.
        [Fact]
        public async Task CreateRewindVaultCertificatesCommand_Sends_MakeVaultCertificateCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;
            const long vaultId = 1;
            var vault = new Vault(vaultId, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, "PLsMroh6zwmH1iU9EjmXXNMivLgqqART1G", 3, 4, 5, 6);

            var certificates = new List<VaultCertificate>
            {
                new VaultCertificate(1, vaultId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", 3, 4, false, false, 5, 10),
                new VaultCertificate(2, vaultId, "PzHqqART1GLsMrwmoh61iU9EjmXXNMivLg", 4, 5, true, true, 6, 10)
            };

            var summaries = new List<VaultContractCertificateSummary>
            {
                new VaultContractCertificateSummary(3, 4, false),
                new VaultContractCertificateSummary(2, 5, false)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(certificates);

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultByIdQuery>(q => q.VaultId == vaultId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vault);

            foreach (var certificate in certificates)
            {
                _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultContractCertificateSummariesByOwnerQuery>(q => q.Vault == vault.Address &&
                                                                                                                         q.Owner == certificate.Owner),
                                                       It.IsAny<CancellationToken>()))
                    // Hack, VaultContractCertificateSummary doesn't reference an Owner because in code, its not needed.
                    // For this, make each certificate a different vested block and use it as an Id. Limits us to 1 cert per owner in this test.
                    .ReturnsAsync(summaries.Where(summary => summary.VestedBlock == certificate.VestedBlock));
            }

            // Act
            try
            {
                await _handler.Handle(new CreateRewindVaultCertificatesCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            foreach (var certificate in certificates)
            {
                var summary = summaries.First(summary => summary.VestedBlock == certificate.VestedBlock);

                certificate.Update(summary, rewindHeight);

                _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultCertificateCommand>(q => q.VaultCertificate == certificate),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
