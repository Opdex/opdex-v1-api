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
    public class CreateWalletRedeemVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateWalletRedeemVaultCertificateCommandHandler _handler;

        public CreateWalletRedeemVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new CreateWalletRedeemVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorMakeRedeemVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";

            // Act
            await _handler.Handle(new CreateWalletRedeemVaultCertificateCommand(walletAddress, vault), new CancellationTokenSource().Token);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeRedeemVaultCertificateCommand>(command => command.WalletAddress == walletAddress
                                                                 && command.Vault == vault),
                CancellationToken.None
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
            var response = await _handler.Handle(new CreateWalletRedeemVaultCertificateCommand(walletAddress, vault), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
