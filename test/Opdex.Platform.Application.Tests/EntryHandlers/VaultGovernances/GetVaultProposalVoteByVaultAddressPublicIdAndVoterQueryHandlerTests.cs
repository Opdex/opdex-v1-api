using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Votes;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances;

public class GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultProposalVote, VaultProposalVoteDto>> _assemblerMock;

    private readonly GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandler _handler;

    public GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultProposalVote, VaultProposalVoteDto>>();

        _handler = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByAddressQuery>(query => query.Vault == request.Vault
                                                                                                      && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveVaultProposalByVaultIdAndPublicIdQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var vote = new VaultProposalVote(14, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vote);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(query => query.VaultId == vault.Id
                                                                                                               && query.PublicId == request.PublicProposalId
                                                                                                               && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var vote = new VaultProposalVote(14, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vote);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery>(query => query.VaultId == vault.Id
                                                                                                                             && query.ProposalId == proposal.Id
                                                                                                                             && query.Voter == request.Voter
                                                                                                                             && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ProposalExists_Assemble()
    {
        // Arrange
        var request = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var vote = new VaultProposalVote(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 100000);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vote);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(vote), Times.Once);
    }

    [Fact]
    public async Task Handle_Assembled_Return()
    {
        // Arrange
        var request = new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var vote = new VaultProposalVote(14, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vote);

        var dto = new VaultProposalVoteDto();
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
    }
}
