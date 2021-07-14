using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class VaultControllerTests
    {
        private const string FakeTransactionEndpoint = "path/to/format/{0}";
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly VaultController _controller;

        public VaultControllerTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            var blockExplorerOptionsMock = new BlockExplorerConfiguration { TransactionEndpoint = FakeTransactionEndpoint };

            _controller = new VaultController(_applicationContextMock.Object, _mapperMock.Object, _mediatorMock.Object, blockExplorerOptionsMock);
        }

        [Fact]
        public async Task GetVaults_GetAllVaultsQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetVaults(cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<GetAllVaultsQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetVaults_GetAllVaultsQueryResponse_Map()
        {
            // Arrange
            var dtos = new VaultDto[] { new VaultDto() };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAllVaultsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dtos);

            // Act
            await _controller.GetVaults(CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<IEnumerable<VaultResponseModel>>(dtos), Times.Once);
        }

        [Fact]
        public async Task GetVaults_GetAllVaultsQueryResponse_ReturnOk()
        {
            // Arrange
            var vaults = new VaultResponseModel[] { new VaultResponseModel() };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAllVaultsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new VaultDto[] { new VaultDto() });
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultResponseModel>>(It.IsAny<IEnumerable<VaultDto>>())).Returns(vaults);

            // Act
            var response = await _controller.GetVaults(CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(vaults);
        }

        [Fact]
        public async Task GetVaultByAddress_GetVaultByAddressQuery_Send()
        {
            // Arrange
            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetVaultByAddress(vaultAddress, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultByAddressQuery>(query => query.Address == vaultAddress),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetVaultByAddress_GetVaultByAddressQueryResponse_Map()
        {
            // Arrange
            var dto = new VaultDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetVaultByAddress("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<VaultResponseModel>(dto), Times.Once);
        }

        [Fact]
        public async Task GetVaultByAddress_Success_ReturnOk()
        {
            // Arrange
            var vault = new VaultResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new VaultDto());
            _mapperMock.Setup(callTo => callTo.Map<VaultResponseModel>(It.IsAny<VaultDto>())).Returns(vault);

            // Act
            var response = await _controller.GetVaultByAddress("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(vault);
        }

        [Fact]
        public async Task SetOwner_ProcessSetVaultOwnerCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var request = new SetVaultOwnerRequest
            {
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.SetOwner(vaultAddress, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWalletSetVaultOwnerCommand>(command
                => command.Vault == vaultAddress
                && command.Owner == request.Owner
                && command.WalletAddress == walletAddress
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SetOwner_Success_ReturnCreated()
        {
            // Arrange
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWalletSetVaultOwnerCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new SetVaultOwnerRequest
            {
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"
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
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var request = new CreateVaultCertificateRequest
            {
                Holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                Amount = "1000.00000000"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.CreateCertificate(vaultAddress, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWalletCreateVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.Holder == request.Holder
                && command.Amount == request.Amount
                && command.WalletAddress == walletAddress
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateCertificate_Success_ReturnCreated()
        {
            // Arrange
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWalletCreateVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new CreateVaultCertificateRequest
            {
                Holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                Amount = "1000.00000000"
            };

            // Act
            var response = await _controller.CreateCertificate("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", request, default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }

        [Fact]
        public async Task RedeemCertificates_ProcessRedeemVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.RedeemCertificates(vaultAddress, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWalletRedeemVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.WalletAddress == walletAddress
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RedeemCertificates_Success_ReturnCreated()
        {
            // Arrange
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWalletRedeemVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            // Act
            var response = await _controller.RedeemCertificates("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                               default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }

        [Fact]
        public async Task RevokeCertificates_ProcessRevokeVaultCertificateCommand_Send()
        {
            // Arrange
            var walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.RevokeCertificates(vaultAddress, new RevokeVaultCertificatesRequest { Holder = holder }, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWalletRevokeVaultCertificateCommand>(command
                => command.Vault == vaultAddress
                && command.Holder == holder
                && command.WalletAddress == walletAddress
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RevokeCertificates_Success_ReturnCreated()
        {
            // Arrange
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            var txId = "j2oD0ma11BkSyx";
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWalletRevokeVaultCertificateCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(txId);

            var request = new RevokeVaultCertificatesRequest { Holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };

            // Act
            var response = await _controller.RevokeCertificates("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                request,
                                                                default);

            // Act
            response.Result.Should().BeOfType<CreatedResult>();
            ((CreatedResult)response.Result).Value.Should().Be(txId);
            ((CreatedResult)response.Result).Location.Should().Be(string.Format(FakeTransactionEndpoint, txId));
        }
    }
}
