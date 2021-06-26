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
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var owner = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";

            // Act
            await _handler.Handle(new ProcessSetVaultOwnerCommand(walletAddress, vault, owner), new CancellationTokenSource().Token);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<MakeSetVaultOwnerCommand>(command => command.WalletAddress == walletAddress
                                                        && command.Vault == vault
                                                        && command.Owner == owner),
                CancellationToken.None
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorMakeSetVaultOwnerCommand_Return()
        {
            // Arrange
            var transactionHash = "047aacf5e43cc8f5ae50f95d986db3c9e41969075d1ccc241184f83fe5962faa";

            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var owner = "PDJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeSetVaultOwnerCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactionHash);

            // Act
            var response = await _handler.Handle(new ProcessSetVaultOwnerCommand(walletAddress, vault, owner), default);

            // Assert
            response.Should().Be(transactionHash);
        }
    }
}
