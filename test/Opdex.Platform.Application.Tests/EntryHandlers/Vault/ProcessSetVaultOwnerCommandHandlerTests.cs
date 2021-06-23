using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using Opdex.Platform.Application.EntryHandlers.Vault;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vault
{
    public class ProcessSetVaultOwnerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProcessSetVaultOwnerCommandHandler _handler;

        public ProcessSetVaultOwnerCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new ProcessSetVaultOwnerCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorMakeSetVaultOwnerCommand_Send()
        {
            // Arrange
            var walletName = "opdex";
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var walletPassword = "5up3rS3cREt";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var owner = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new ProcessSetVaultOwnerCommand(walletName, walletAddress, walletPassword, vault, owner), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeSetVaultOwnerCommand>(command => command.WalletName == walletName
                                                        && command.WalletAddress == walletAddress
                                                        && command.WalletPassword == walletPassword
                                                        && command.Vault == vault
                                                        && command.Owner == owner),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorMakeSetVaultOwnerCommand_Return()
        {
            // Arrange
            var transactionHash = "047aacf5e43cc8f5ae50f95d986db3c9e41969075d1ccc241184f83fe5962faa";

            var walletName = "opdex";
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var walletPassword = "5up3rS3cREt";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var owner = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeSetVaultOwnerCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactionHash);

            // Act
            var response = await _handler.Handle(new ProcessSetVaultOwnerCommand(walletName, walletAddress, walletPassword, vault, owner), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
