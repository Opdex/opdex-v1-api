using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class VaultGovernancesControllerTests
{
    private readonly Mock<IApplicationContext> _applicationContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly VaultGovernancesController _controller;

    public VaultGovernancesControllerTests()
    {
        _applicationContextMock = new Mock<IApplicationContext>();
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _controller = new VaultGovernancesController(_applicationContextMock.Object, _mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetVault_GetVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        Address vault = new("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5");

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.GetVault(vault, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultGovernanceByAddressQuery>(query => query.Vault == vault), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetVault_VaultExists_MapResult()
    {
        // Arrange
        var dto = new VaultGovernanceDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetVault(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<VaultGovernanceResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task GetVault_VaultExists_Return200Ok()
    {
        // Arrange
        var expectedResponse = new VaultGovernanceResponseModel();
        var dto = new VaultGovernanceDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<VaultGovernanceResponseModel>(dto)).Returns(expectedResponse);

        // Act
        var response = await _controller.GetVault(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task ProposeCreateCertificateQuote_CreateVaultProposalCreateCertificateQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Amount = FixedDecimal.Parse("500.00000000"),
            Description = "OVP-1: Create certificate."
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteProposeCreateCertificate(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalCreateCertificateQuoteCommand>(command => command.Vault == vault
                                                                                                                   && command.Owner == request.Owner
                                                                                                                   && command.Amount == request.Amount
                                                                                                                   && command.Description == request.Description
                                                                                                                   && command.WalletAddress == walletAddress
                                                                                                          ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProposeCreateCertificateQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalCreateCertificateQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Amount = FixedDecimal.Parse("500.00000000"),
            Description = "OVP-1: Create certificate."
        };

        // Act
        try
        {
            await _controller.QuoteProposeCreateCertificate(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task ProposeCreateCertificateQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalCreateCertificateQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Amount = FixedDecimal.Parse("500.00000000"),
            Description = "OVP-1: Create certificate."
        };

        // Act
        var response = await _controller.QuoteProposeCreateCertificate(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task ProposeRevokeCertificateQuote_RevokeVaultProposalRevokeCertificateQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Description = "OVP-1: Revoke certificate."
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteProposeRevokeCertificate(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalRevokeCertificateQuoteCommand>(command => command.Vault == vault
                                                                                                                   && command.Owner == request.Owner
                                                                                                                   && command.Description == request.Description
                                                                                                                   && command.WalletAddress == walletAddress
                                                                                                          ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProposeRevokeCertificateQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalRevokeCertificateQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Description = "OVP-1: Revoke certificate."
        };

        // Act
        try
        {
            await _controller.QuoteProposeRevokeCertificate(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task ProposeRevokeCertificateQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalRevokeCertificateQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Owner = new("PRWsLtqy7ArRoi8pCeF8VWcYwBaSp6FacW"),
            Description = "OVP-1: Revoke certificate."
        };

        // Act
        var response = await _controller.QuoteProposeRevokeCertificate(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task ProposeMinimumPledgeQuote_CreateVaultProposalMinimumPledgeQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        var request = new MinimumPledgeVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum pledge."
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteProposeMinimumPledge(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalMinimumPledgeQuoteCommand>(command => command.Vault == vault
                                                                                                               && command.Amount == request.Amount
                                                                                                               && command.Description == request.Description
                                                                                                               && command.WalletAddress == walletAddress
                                                                                                       ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProposeMinimumPledgeQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalMinimumPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new MinimumPledgeVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum pledge."
        };

        // Act
        try
        {
            await _controller.QuoteProposeMinimumPledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task ProposeMinimumPledgeQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalMinimumPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new MinimumPledgeVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum pledge."
        };

        // Act
        var response = await _controller.QuoteProposeMinimumPledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task ProposeMinimumVoteQuote_CreateVaultProposalMinimumVoteQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        var request = new MinimumVoteVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum vote."
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteProposeMinimumVote(vault, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalMinimumVoteQuoteCommand>(command => command.Vault == vault
                                                                                                             && command.Amount == request.Amount
                                                                                                             && command.Description == request.Description
                                                                                                             && command.WalletAddress == walletAddress
                                                                                                     ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProposeMinimumVoteQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalMinimumVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new MinimumVoteVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum vote."
        };

        // Act
        try
        {
            await _controller.QuoteProposeMinimumVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task ProposeMinimumVoteQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalMinimumVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new MinimumVoteVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("25500.00000000"),
            Description = "OVP-1: Change minimum vote."
        };

        // Act
        var response = await _controller.QuoteProposeMinimumVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task GetProposal_GetVaultProposalByVaultAddressAndPublicIdQuery_Send()
    {
        // Arrange
        Address vault = new("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5");
        ulong proposalId = 5;

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.GetProposal(vault, proposalId, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultProposalByVaultAddressAndPublicIdQuery>(query => query.Vault == vault
                                                                                                               && query.PublicProposalId == proposalId), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetProposal_ProposalExists_MapResult()
    {
        // Arrange
        var dto = new VaultProposalDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalByVaultAddressAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetProposal(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<VaultProposalResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task GetProposal_ProposalExists_Return200Ok()
    {
        // Arrange
        var expectedResponse = new VaultProposalResponseModel();
        var dto = new VaultProposalDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalByVaultAddressAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<VaultProposalResponseModel>(dto)).Returns(expectedResponse);

        // Act
        var response = await _controller.GetProposal(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetVote_GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery_Send()
    {
        // Arrange
        Address vault = new("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5");
        ulong proposalId = 5;
        Address voter = new("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym");

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.GetVote(vault, proposalId, voter, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery>(query => query.Vault == vault
                                                                                                                          && query.PublicProposalId == proposalId
                                                                                                                          && query.Voter == voter
                                                                                                                 ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetVote_ProposalVoteExists_MapResult()
    {
        // Arrange
        var dto = new VaultProposalVoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetVote(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym"), CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<VaultProposalVoteResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task GetVote_ProposalVoteExists_Return200Ok()
    {
        // Arrange
        var expectedResponse = new VaultProposalVoteResponseModel();
        var dto = new VaultProposalVoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<VaultProposalVoteResponseModel>(dto)).Returns(expectedResponse);

        // Act
        var response = await _controller.GetVote(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym"), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetPledge_GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery_Send()
    {
        // Arrange
        Address vault = new("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5");
        ulong proposalId = 5;
        Address voter = new("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym");

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.GetPledge(vault, proposalId, voter, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery>(query => query.Vault == vault
                                                                                                                            && query.PublicProposalId == proposalId
                                                                                                                            && query.Pledger == voter
                                                                                                                     ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetPledge_ProposalPledgeExists_MapResult()
    {
        // Arrange
        var dto = new VaultProposalPledgeDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetPledge(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym"), CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<VaultProposalPledgeResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task GetPledge_ProposalPledgeExists_Return200Ok()
    {
        // Arrange
        var expectedResponse = new VaultProposalPledgeResponseModel();
        var dto = new VaultProposalPledgeDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<VaultProposalPledgeResponseModel>(dto)).Returns(expectedResponse);

        // Act
        var response = await _controller.GetPledge(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PWWs7mBQKJzyMFiPzV7KhopQXPijqwHNym"), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task PledgeQuote_CreateVaultProposalPledgeQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        ulong proposalId = 5;
        var request = new VaultProposalPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuotePledge(vault, proposalId, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalPledgeQuoteCommand>(command => command.Vault == vault
                                                                                                        && command.ProposalId == proposalId
                                                                                                        && command.Amount == request.Amount
                                                                                                        && command.WalletAddress == walletAddress
                                                                                               ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task PledgeQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new VaultProposalPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        try
        {
            await _controller.QuotePledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task PledgeQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new VaultProposalPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        var response = await _controller.QuotePledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task WithdrawPledgeQuote_CreateWithdrawVaultProposalPledgeQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        ulong proposalId = 5;
        var request = new VaultProposalWithdrawPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteWithdrawPledge(vault, proposalId, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWithdrawVaultProposalPledgeQuoteCommand>(command => command.Vault == vault
                                                                                                                && command.ProposalId == proposalId
                                                                                                                && command.Amount == request.Amount
                                                                                                                && command.WalletAddress == walletAddress
                                                                                                       ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task WithdrawPledgeQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWithdrawVaultProposalPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new VaultProposalWithdrawPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        try
        {
            await _controller.QuoteWithdrawPledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task WithdrawPledgeQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWithdrawVaultProposalPledgeQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new VaultProposalWithdrawPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        var response = await _controller.QuoteWithdrawPledge(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task VoteQuote_CreateVaultProposalVoteQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        ulong proposalId = 5;
        var request = new VaultProposalVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000"),
            InFavor = true
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteVote(vault, proposalId, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateVaultProposalVoteQuoteCommand>(command => command.Vault == vault
                                                                                                        && command.ProposalId == proposalId
                                                                                                        && command.Amount == request.Amount
                                                                                                        && command.InFavor == request.InFavor
                                                                                                        && command.WalletAddress == walletAddress
                                                                                               ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task VoteQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new VaultProposalVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000"),
            InFavor = true
        };

        // Act
        try
        {
            await _controller.QuoteVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task VoteQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateVaultProposalVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new VaultProposalVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000"),
            InFavor = true
        };

        // Act
        var response = await _controller.QuoteVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }

    [Fact]
    public async Task WithdrawVoteQuote_CreateWithdrawVaultProposalVoteQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        Address vault = new("PR71udY85pAcNcitdDfzQevp6Zar9DizHM");
        ulong proposalId = 5;
        var request = new VaultProposalWithdrawVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.QuoteWithdrawVote(vault, proposalId, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateWithdrawVaultProposalVoteQuoteCommand>(command => command.Vault == vault
                                                                                                                && command.ProposalId == proposalId
                                                                                                                && command.Amount == request.Amount
                                                                                                                && command.WalletAddress == walletAddress
                                                                                                       ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task WithdrawVoteQuote_Result_Map()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var quote = new TransactionQuoteDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWithdrawVaultProposalVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(quote);

        var request = new VaultProposalWithdrawVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        try
        {
            await _controller.QuoteWithdrawVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
    }

    [Fact]
    public async Task WithdrawVoteQuote_Success_ReturnOk()
    {
        // Arrange
        Address walletAddress = new("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");
        _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

        var responseModel = new TransactionQuoteResponseModel();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateWithdrawVaultProposalVoteQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

        var request = new VaultProposalWithdrawVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("500.00000000")
        };

        // Act
        var response = await _controller.QuoteWithdrawVote(new Address("PR71udY85pAcNcitdDfzQevp6Zar9DizHM"), 5, request, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
    }
}
