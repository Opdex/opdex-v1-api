using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Responses.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class VaultGovernancesControllerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly VaultGovernancesController _controller;

    public VaultGovernancesControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _controller = new VaultGovernancesController(_mapperMock.Object, _mediatorMock.Object);
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
}
