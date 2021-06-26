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
    public class ProcessCreateVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProcessCreateVaultCertificateCommandHandler _handler;

        public ProcessCreateVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new ProcessCreateVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorMakeCreateVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var holder = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var amount = "10000000000";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new ProcessCreateVaultCertificateCommand(walletAddress, vault, holder, amount), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeCreateVaultCertificateCommand>(command => command.WalletAddress == walletAddress
                                                                 && command.Vault == vault
                                                                 && command.Holder == holder
                                                                 && command.Amount == amount),
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
            var holder = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var amount = "10000000000";

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeCreateVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactionHash);

            // Act
            var response = await _handler.Handle(new ProcessCreateVaultCertificateCommand(walletAddress, vault, holder, amount), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
