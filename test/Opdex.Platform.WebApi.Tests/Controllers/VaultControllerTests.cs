using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using Opdex.Platform.Common;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Vault;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class VaultControllerTests
    {
        private const string FakeTransactionEndpoint = "path/to/format/{0}";
        private readonly Mock<IMediator> _mediatorMock;
        private readonly VaultController _controller;

        public VaultControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            var blockExplorerOptionsMock = new Mock<IOptions<BlockExplorerConfiguration>>();
            blockExplorerOptionsMock.Setup(callTo => callTo.Value).Returns(new BlockExplorerConfiguration
            {
                TransactionEndpoint = FakeTransactionEndpoint
            });
            _controller = new VaultController(_mediatorMock.Object, blockExplorerOptionsMock.Object);
        }

        [Fact]
        public async Task SetOwner_ProcessSetVaultOwnerCommand_Send()
        {
            // Arrange
            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var request = new SetVaultOwnerRequest
            {
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.SetOwner(vaultAddress, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<ProcessSetVaultOwnerCommand>(command
                => command.Vault == vaultAddress
                && command.Owner == request.Owner
                && command.WalletAddress == request.WalletAddress
                && command.WalletName == request.WalletName
                && command.WalletPassword == request.WalletPassword
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SetOwner_Success_ReturnCreated()
        {
            // Arrange
            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<ProcessSetVaultOwnerCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new SetVaultOwnerRequest
            {
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };

            // Act
            var response = await _controller.SetOwner("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", request, default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }

        [Fact]
        public async Task CreateCertificate_ProcessCreateVaultCertificateCommand_Send()
        {
            // Arrange
            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var request = new CreateVaultCertificateRequest
            {
                Holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                Amount = "1000000000",
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.CreateCertificate(vaultAddress, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<ProcessCreateVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.Holder == request.Holder
                && command.Amount == request.Amount
                && command.WalletAddress == request.WalletAddress
                && command.WalletName == request.WalletName
                && command.WalletPassword == request.WalletPassword
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateCertificate_Success_ReturnCreated()
        {
            // Arrange
            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<ProcessCreateVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new CreateVaultCertificateRequest
            {
                Holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                Amount = "1000000000",
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };

            // Act
            var response = await _controller.CreateCertificate("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", request, default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }

        [Fact]
        public async Task RedeemCertificate_ProcessRedeemVaultCertificateCommand_Send()
        {
            // Arrange
            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new RedeemVaultCertificateRequest
            {
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.RedeemCertificate(vaultAddress, holder, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<ProcessRedeemVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.Holder == holder
                && command.WalletAddress == request.WalletAddress
                && command.WalletName == request.WalletName
                && command.WalletPassword == request.WalletPassword
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RedeemCertificate_Success_ReturnCreated()
        {
            // Arrange
            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<ProcessRedeemVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new RedeemVaultCertificateRequest
            {
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };

            // Act
            var response = await _controller.RedeemCertificate("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                               "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXm",
                                                               request,
                                                               default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }

        [Fact]
        public async Task RevokeCertificate_ProcessRevokeVaultCertificateCommand_Send()
        {
            // Arrange
            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new RevokeVaultCertificateRequest
            {
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.RevokeCertificate(vaultAddress, holder, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<ProcessRevokeVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.Holder == holder
                && command.WalletAddress == request.WalletAddress
                && command.WalletName == request.WalletName
                && command.WalletPassword == request.WalletPassword
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RevokeCertificate_Success_ReturnCreated()
        {
            // Arrange
            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<ProcessRevokeVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new RevokeVaultCertificateRequest
            {
                WalletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                WalletName = "opdex",
                WalletPassword = "pwd"
            };

            // Act
            var response = await _controller.RevokeCertificate("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                               "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                                                               request,
                                                               default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }
    }
}
