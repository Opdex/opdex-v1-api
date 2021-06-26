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
    public class MakeRedeemVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MakeRedeemVaultCertificateCommandHandler _handler;

        public MakeRedeemVaultCertificateCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new MakeRedeemVaultCertificateCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_CallCirrusCallSmartContractMethodCommand_Send()
        {
            // Arrange
            var request = new MakeRedeemVaultCertificateCommand(walletAddress: "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                vault: "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            // Act
            await _handler.Handle(request, new CancellationTokenSource().Token);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusCallSmartContractMethodCommand>(
                command => command.CallDto.ContractAddress == request.Vault
                        && command.CallDto.WalletName == request.WalletName
                        && command.CallDto.Sender == request.WalletAddress
                        && command.CallDto.Password == request.WalletPassword
                        && command.CallDto.Amount == "0"
                        && command.CallDto.MethodName == "RedeemCertificates"
                        && command.CallDto.Parameters == null
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
            var response = await _handler.Handle(new MakeRedeemVaultCertificateCommand("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                                       "PCJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk"), default);

            // Assert
            response.Should().Be(txId);
        }
    }
}
