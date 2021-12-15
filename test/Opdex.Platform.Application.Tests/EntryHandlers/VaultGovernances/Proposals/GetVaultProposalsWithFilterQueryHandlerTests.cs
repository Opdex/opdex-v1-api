using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Proposals;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Proposals;

public class GetVaultProposalsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultProposal, VaultProposalDto>> _assemblerMock;

    private readonly GetVaultProposalsWithFilterQueryHandler _handler;

    public GetVaultProposalsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultProposal, VaultProposalDto>>();

        _handler = new GetVaultProposalsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, default, default, default, default);
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByAddressQuery>(query => query.Vault == vaultAddress
                                                                                                      && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveVaultProposalsWithFilterQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, default, default, default, default);
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);
        var cancellationToken = new CancellationTokenSource().Token;

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalsWithFilterQuery>(query => query.VaultId == vault.Id
                                                                                                      && query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ProposalsRetrieved_MapResults()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, default, default, default, default);
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<VaultProposal>()), Times.Exactly(proposals.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 3, PagingDirection.Backward, (300000, 55));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Proposals.Count().Should().Be(proposals.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (300000, 55));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(proposals[0]), Times.Never);
        dto.Proposals.Count().Should().Be(proposals.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (300000, 55));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(proposals[proposals.Length - 1]), Times.Never);
        dto.Proposals.Count().Should().Be(proposals.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (default, default));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (proposals[^2].Expiration, proposals[^2].PublicId));
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (300000, 50));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (proposals[^2].Expiration, proposals[^2].PublicId));
        AssertPrevious(dto.Cursor, (proposals[0].Expiration, proposals[0].PublicId));
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (300000, 50));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(15, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (proposals[^1].Expiration, proposals[^1].PublicId));
        AssertPrevious(dto.Cursor, (proposals[1].Expiration, proposals[1].PublicId));
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Forward, (300000, 50));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, (proposals[0].Expiration, proposals[0].PublicId));
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalsCursor(default, default, SortDirectionType.ASC, 2, PagingDirection.Backward, (300000, 50));
        var request = new GetVaultProposalsWithFilterQuery(vaultAddress, cursor);

        var proposals = new[]
        {
            new VaultProposal(5, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500),
            new VaultProposal(10, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000, "Proposal description", VaultProposalType.Revoke,
                              VaultProposalStatus.Pledge, 100000, 125000000, 40000000, 230000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(proposals);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposal>())).ReturnsAsync(new VaultProposalDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, (proposals[^1].Expiration, proposals[^1].PublicId));
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, (ulong, ulong) pointer)
    {
        VaultProposalsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, (ulong, ulong) pointer)
    {
        VaultProposalsCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
