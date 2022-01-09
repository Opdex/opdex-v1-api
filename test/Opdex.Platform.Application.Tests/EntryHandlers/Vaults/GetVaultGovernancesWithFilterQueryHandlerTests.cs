using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults;

public class GetVaultGovernancesWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<VaultGovernance, VaultGovernanceDto>> _assemblerMock;

    private readonly GetVaultGovernancesWithFilterQueryHandler _handler;

    public GetVaultGovernancesWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<VaultGovernance, VaultGovernanceDto>>();

        _handler = new GetVaultGovernancesWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object, new NullLogger<GetVaultGovernancesWithFilterQueryHandler>());
    }

    [Fact]
    public async Task Handle_RetrieveVaultGovernancesWithFilterQuery_Send()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);
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
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernancesWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VaultsRetrieved_MapResults()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<VaultGovernance>()), Times.Exactly(vaults.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Vaults.Count().Should().Be(vaults.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(vaults[0]), Times.Never);
        dto.Vaults.Count().Should().Be(vaults.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(vaults[vaults.Length - 1]), Times.Never);
        dto.Vaults.Count().Should().Be(vaults.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, vaults[^2].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, vaults[^2].Id);
        AssertPrevious(dto.Cursor, vaults[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(15, "PTnQShk1J2ids8xZs2ELG8Xj2HzWbxoW2r", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, vaults[^1].Id);
        AssertPrevious(dto.Cursor, vaults[1].Id);
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, vaults[0].Id);
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new VaultGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultGovernancesWithFilterQuery(cursor);

        var vaults = new []
        {
            new VaultGovernance(5, "PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
            new VaultGovernance(10, "PKHztTdYwqeCVC2MzhPMh7ESGWBhj4i8TT", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500),
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<VaultGovernance>())).ReturnsAsync(new VaultGovernanceDto());

        var vault = new VaultGovernance(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, vaults[^1].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, ulong pointer)
    {
        VaultGovernancesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, ulong pointer)
    {
        VaultGovernancesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
