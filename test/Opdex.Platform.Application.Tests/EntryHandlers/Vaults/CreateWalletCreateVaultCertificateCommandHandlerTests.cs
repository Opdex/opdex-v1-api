using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class CreateWalletCreateVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateWalletCreateVaultCertificateCommandHandler _handler;

        public CreateWalletCreateVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new CreateWalletCreateVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorMakeCreateVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var holder = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var amount = "1000.00000000";

            // Act
            await _handler.Handle(new CreateWalletCreateVaultCertificateCommand(walletAddress, vault, holder, amount),
                                  new CancellationTokenSource().Token);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeWalletCreateVaultCertificateCommand>(command => command.WalletAddress == walletAddress
                                                                       && command.Vault == vault
                                                                       && command.Holder == holder
                                                                       && command.Amount == amount.ToSatoshis(TokenConstants.Opdex.Decimals)),
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
            var holder = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var amount = "1000.00000000";

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeWalletCreateVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactionHash);

            // Act
            var response = await _handler.Handle(new CreateWalletCreateVaultCertificateCommand(walletAddress, vault, holder, amount), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
