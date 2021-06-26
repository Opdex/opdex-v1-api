using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Vaults
{
    public class MakeCreateVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MakeCreateVaultCertificateCommandHandler _handler;

        public MakeCreateVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new MakeCreateVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_CallCirrusCallSmartContractMethodCommand_Send()
        {
            // Arrange
            var request = new MakeCreateVaultCertificateCommand(walletAddress: "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                vault: "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                                holder: "PFJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                                                                amount: "10000000");

            // Act
            await _handler.Handle(request, new CancellationTokenSource().Token);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusCallSmartContractMethodCommand>(
                command => command.CallDto.ContractAddress == request.Vault
                        && command.CallDto.WalletName == request.WalletName
                        && command.CallDto.Sender == request.WalletAddress
                        && command.CallDto.Password == request.WalletPassword
                        && command.CallDto.Amount == "0"
                        && command.CallDto.MethodName == "CreateCertificate"
                        && command.CallDto.Parameters[0] == $"9#{request.Holder}"
                        && command.CallDto.Parameters[1] == $"12#{request.Amount}"
            ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_CallCirrusCallSmartContractMethodCommand_Return()
        {
            // Arrange
            var txId = "047aacf5e43cc8f5ae50f95d986db3c9e41969075d1ccc241184f83fe5962faa";


            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusCallSmartContractMethodCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            // Act
            var response = await _handler.Handle(new MakeCreateVaultCertificateCommand("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                                       "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                                                       "PFJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                                                                                       "1000000"), default);

            // Assert
            response.Should().Be(txId);
        }
    }
}
