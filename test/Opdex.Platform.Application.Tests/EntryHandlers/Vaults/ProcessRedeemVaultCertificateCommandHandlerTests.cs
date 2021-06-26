using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class ProcessRedeemVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProcessRedeemVaultCertificateCommandHandler _handler;

        public ProcessRedeemVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new ProcessRedeemVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorMakeRedeemVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new ProcessRedeemVaultCertificateCommand(walletAddress, vault), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeRedeemVaultCertificateCommand>(command => command.WalletAddress == walletAddress
                                                                 && command.Vault == vault),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorPersistMarketPermissionCommand_Return()
        {
            // Arrange
            var transactionHash = "047aacf5e43cc8f5ae50f95d986db3c9e41969075d1ccc241184f83fe5962faa";

            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeRedeemVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactionHash);

            // Act
            var response = await _handler.Handle(new ProcessRedeemVaultCertificateCommand(walletAddress, vault), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
