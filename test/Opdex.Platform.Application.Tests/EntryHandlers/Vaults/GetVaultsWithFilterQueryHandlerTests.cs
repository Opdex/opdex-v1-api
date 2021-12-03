using FluentAssertions;
using MediatR;
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
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults;

public class GetVaultsWithFilterQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<Vault, VaultDto>> _assemblerMock;

    private readonly GetVaultsWithFilterQueryHandler _handler;

    public GetVaultsWithFilterQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<Vault, VaultDto>>();

        _handler = new GetVaultsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveVaultsWithFilterQuery_Send()
    {
        // Arrange
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultsWithFilterQuery(cursor);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultsWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VaultsRetrieved_MapResults()
    {
        // Arrange
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<Vault>()), Times.Exactly(vaults.Length));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Vaults.Count().Should().Be(vaults.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

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
        var cursor = new VaultsCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetVaultsWithFilterQuery(cursor);

        var vaults = new Vault[]
        {
            new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505),
            new Vault(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, Address.Empty, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vaults);
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(new VaultDto());

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, vaults[^1].Id);
        dto.Cursor.Previous.Should().Be(null);
    }

    private void AssertNext(CursorDto dto, ulong pointer)
    {
        VaultsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private void AssertPrevious(CursorDto dto, ulong pointer)
    {
        VaultsCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}