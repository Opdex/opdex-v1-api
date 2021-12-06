using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances;

public class GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>> _assemblerMock;

    private readonly GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandler _handler;

    public GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>>();

        _handler = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

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
        var request = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var pledge = new VaultProposalPledge(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledge);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(query => query.VaultId == vault.Id
                                                                                                               && query.PublicId == request.PublicProposalId
                                                                                                               && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var request = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var pledge = new VaultProposalPledge(5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 100000);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledge);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery>(query => query.VaultId == vault.Id
                                                                                                                                 && query.ProposalId == proposal.Id
                                                                                                                                 && query.Pledger == request.Pledger
                                                                                                                                 && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ProposalExists_Assemble()
    {
        // Arrange
        var request = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var pledge = new VaultProposalPledge(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledge);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(pledge), Times.Once);
    }

    [Fact]
    public async Task Handle_Assembled_Return()
    {
        // Arrange
        var request = new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), 5, new Address("PKQc4Lj7pTy9se1B3qBLDQv33nZ34bnSP1"));

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        var proposal = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalByVaultIdAndPublicIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposal);

        var pledge = new VaultProposalPledge(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledge);

        var dto = new VaultProposalPledgeDto();
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(dto);
    }
}
