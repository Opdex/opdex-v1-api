using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Pledges;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Pledges;

public class GetVaultProposalPledgesWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>> _assemblerMock;

    private readonly GetVaultProposalPledgesWithFilterQueryHandler _handler;

    public GetVaultProposalPledgesWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>>();

        _handler = new GetVaultProposalPledgesWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object, new NullLogger<GetVaultProposalPledgesWithFilterQueryHandler>());
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByAddressQuery>(query => query.Vault == vaultAddress
                                                                                                      && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveVaultProposalPledgesWithFilterQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);
        var cancellationToken = new CancellationTokenSource().Token;

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalPledgesWithFilterQuery>(query => query.VaultId == vault.Id
                                                                                                            && query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VaultsRetrieved_MapResults()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>()), Times.Exactly(pledges.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Pledges.Count().Should().Be(pledges.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(pledges[0]), Times.Never);
        dto.Pledges.Count().Should().Be(pledges.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(pledges[pledges.Length - 1]), Times.Never);
        dto.Pledges.Count().Should().Be(pledges.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, pledges[^2].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, pledges[^2].Id);
        AssertPrevious(dto.Cursor, pledges[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, pledges[^1].Id);
        AssertPrevious(dto.Cursor, pledges[1].Id);
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, pledges[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalPledgesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultProposalPledgesWithFilterQuery(vaultAddress, cursor);

        var pledges = new VaultProposalPledge[]
        {
            new VaultProposalPledge(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500),
            new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalPledgesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pledges);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalPledge>())).ReturnsAsync(new VaultProposalPledgeDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, pledges[^1].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, ulong pointer)
    {
        VaultProposalPledgesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, ulong pointer)
    {
        VaultProposalPledgesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
