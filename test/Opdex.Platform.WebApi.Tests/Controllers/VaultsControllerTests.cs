using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class VaultsControllerTests
{
    private readonly Mock<IApplicationContext> _applicationContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly VaultsController _controller;

    public VaultsControllerTests()
    {
        _applicationContextMock = new Mock<IApplicationContext>();
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _controller = new VaultsController(_applicationContextMock.Object, _mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetVaults_GetVaultsWithFilterQuery_Send()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetVaults(new VaultFilterParameters(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultsWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetVaults_Result_ReturnOk()
    {
        // Arrange
        var vaults = new VaultsResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new VaultsDto());
        _mapperMock.Setup(callTo => callTo.Map<VaultsResponseModel>(It.IsAny<VaultsDto>())).Returns(vaults);

        // Act
        var response = await _controller.GetVaults(new VaultFilterParameters(), CancellationToken.None);

        // Assert
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
    public async Task SetOwnerQuote_CreateSetPendingVaultOwnershipTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var request = new SetVaultOwnerQuoteRequest
        {
            Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.SetOwnerQuote(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateSetPendingVaultOwnershipTransactionQuoteCommand>(command
                                                                                                                    => command.Vault == vault
                                                                                                                       && command.WalletAddress == walletAddress
                                                                                                                       && command.NewOwner == request.Owner
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task SetOwnerQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetPendingVaultOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var request = new SetVaultOwnerQuoteRequest
        {
            Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        try
        {
            await _controller.SetOwnerQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task SetOwnerQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetPendingVaultOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new SetVaultOwnerQuoteRequest
        {
            Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        var response = await _controller.SetOwnerQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task ClaimOwnershipQuote_CreateClaimPendingVaultOwnershipTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.ClaimOwnershipQuote(vault, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateClaimPendingVaultOwnershipTransactionQuoteCommand>(command
                                                                                                                      => command.Vault == vault
                                                                                                                         && command.WalletAddress == walletAddress
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ClaimOwnershipQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateClaimPendingVaultOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        // Act
        try
        {
            await _controller.ClaimOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task ClaimOwnershipQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateClaimPendingVaultOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        // Act
        var response = await _controller.ClaimOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task GetVaultCertificates_GetVaultCertificatesWithFilterQuery_Send()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetVaultCertificates("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i", new VaultCertificateFilterParameters(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultCertificatesWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetVaultCertificates_Result_ReturnOk()
    {
        // Arrange
        var certificates = new VaultCertificatesResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new VaultCertificatesDto());
        _mapperMock.Setup(callTo => callTo.Map<VaultCertificatesResponseModel>(It.IsAny<VaultCertificatesDto>())).Returns(certificates);

        // Act
        var response = await _controller.GetVaultCertificates("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i", new VaultCertificateFilterParameters(), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(certificates);
    }

    [Fact]
    public async Task CreateCertificateQuote_CreateCreateVaultCertificateTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vaultAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var request = new CreateVaultCertificateQuoteRequest
        {
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm",
            Amount = FixedDecimal.Parse("10000.00000000")
        };
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.CreateCertificateQuote(vaultAddress, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateCreateVaultCertificateTransactionQuoteCommand>(command
                                                                                                                  => command.Vault == vaultAddress
                                                                                                                     && command.WalletAddress == walletAddress
                                                                                                                     && command.Holder == request.Holder
                                                                                                                     && command.Amount == request.Amount
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateCertificateQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateVaultCertificateTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var request = new CreateVaultCertificateQuoteRequest
        {
            Amount = FixedDecimal.Parse("1000.00000000"),
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        try
        {
            await _controller.CreateCertificateQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task CreateCertificateQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateVaultCertificateTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new CreateVaultCertificateQuoteRequest
        {
            Amount = FixedDecimal.Parse("1000.00000000"),
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        var response = await _controller.CreateCertificateQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task RedeemCertificatesQuote_CreateRedeemVaultCertificatesTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.RedeemCertificatesQuote(vault, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateRedeemVaultCertificatesTransactionQuoteCommand>(command
                                                                                                                   => command.Vault == vault
                                                                                                                      && command.WalletAddress == walletAddress
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RedeemCertificatesQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRedeemVaultCertificatesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        // Act
        try
        {
            await _controller.RedeemCertificatesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task RedeemCertificatesQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRedeemVaultCertificatesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        // Act
        var response = await _controller.RedeemCertificatesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task RevokeCertificatesQuote_CreateRevokeVaultCertificatesTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var request = new RevokeVaultCertificatesQuoteRequest
        {
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.RevokeCertificatesQuote(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateRevokeVaultCertificatesTransactionQuoteCommand>(command
                                                                                                                   => command.Vault == vault
                                                                                                                      && command.WalletAddress == walletAddress
                                                                                                                      && command.Holder == request.Holder
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RevokeCertificatesQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRevokeVaultCertificatesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var request = new RevokeVaultCertificatesQuoteRequest
        {
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        try
        {
            await _controller.RevokeCertificatesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task RevokeCertificatesQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRevokeVaultCertificatesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new RevokeVaultCertificatesQuoteRequest
        {
            Holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
        };

        // Act
        var response = await _controller.RevokeCertificatesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }
}