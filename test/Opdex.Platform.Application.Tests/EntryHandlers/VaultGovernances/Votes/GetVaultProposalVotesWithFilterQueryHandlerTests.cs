using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances.Votes;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances.Votes;

public class GetVaultProposalVotesWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultProposalVote, VaultProposalVoteDto>> _assemblerMock;

    private readonly GetVaultProposalVotesWithFilterQueryHandler _handler;

    public GetVaultProposalVotesWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultProposalVote, VaultProposalVoteDto>>();

        _handler = new GetVaultProposalVotesWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernanceByAddressQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);
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
    public async Task Handle_RetrieveVaultProposalVotesWithFilterQuery_Send()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);
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
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultProposalVotesWithFilterQuery>(query => query.VaultId == vault.Id
                                                                                                          && query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VaultsRetrieved_MapResults()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>()), Times.Exactly(votes.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Votes.Count().Should().Be(votes.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(votes[0]), Times.Never);
        dto.Votes.Count().Should().Be(votes.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(votes[votes.Length - 1]), Times.Never);
        dto.Votes.Count().Should().Be(votes.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, votes[^2].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, votes[^2].Id);
        AssertPrevious(dto.Cursor, votes[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(15, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, votes[^1].Id);
        AssertPrevious(dto.Cursor, votes[1].Id);
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, votes[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var vaultAddress = new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i");
        var cursor = new VaultProposalVotesCursor(5, Address.Empty, true, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultProposalVotesWithFilterQuery(vaultAddress, cursor);

        var votes = new[]
        {
            new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500),
            new VaultProposalVote(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultProposalVotesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(votes);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultProposalVote>())).ReturnsAsync(new VaultProposalVoteDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, votes[^1].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, ulong pointer)
    {
        VaultProposalVotesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, ulong pointer)
    {
        VaultProposalVotesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
