using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks;

public class GetBlocksWithFilterQueryHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;

    private readonly GetBlocksWithFilterQueryHandler _handler;

    public GetBlocksWithFilterQueryHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new GetBlocksWithFilterQueryHandler(
            new NullLogger<GetBlocksWithFilterQueryHandler>(), _mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveBlocksWithFilterQuery_Send()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetBlocksWithFilterQuery(cursor);
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
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveBlocksWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_BlocksRetrieved_MapResults()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
        var request = new GetBlocksWithFilterQuery(cursor);

        var block = new Block(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var blocks = new[] { block };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<IEnumerable<BlockDto>>(blocks));
    }

    [Fact]
    public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Blocks.Count().Should().Be(blocks.Length);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<IEnumerable<BlockDto>>(
            It.Is<IEnumerable<Block>>(s => s.SequenceEqual(blocks.Skip(1)))), Times.Once);
        dto.Blocks.Count().Should().Be(blocks.Length - 1);
    }

    [Fact]
    public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<IEnumerable<BlockDto>>(
            It.Is<IEnumerable<Block>>(s => s.SequenceEqual(blocks.Take(blocks.Length - 1)))), Times.Once);
        dto.Blocks.Count().Should().Be(blocks.Length - 1);
    }

    [Fact]
    public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, blocks[^2].Height);
        dto.Cursor.Previous.Should().Be(null);
    }

    [Fact]
    public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, blocks[^2].Height);
        AssertPrevious(dto.Cursor, blocks[0].Height);
    }

    [Fact]
    public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(15, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, blocks[^1].Height);
        AssertPrevious(dto.Cursor, blocks[1].Height);
    }

    [Fact]
    public async Task Handle_PagingForwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        dto.Cursor.Next.Should().Be(null);
        AssertPrevious(dto.Cursor, blocks[0].Height);
    }

    [Fact]
    public async Task Handle_PagingBackwardLastPage_ReturnCursor()
    {
        // Arrange
        var cursor = new BlocksCursor(SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
        var request = new GetBlocksWithFilterQuery(cursor);

        var blocks = new Block[]
        {
            new(5, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow),
            new(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow)
        };
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blocks);
        _mapperMock.Setup(callTo => callTo.Map<IEnumerable<BlockDto>>(It.IsAny<IEnumerable<Block>>()))
            .Returns<IEnumerable<Block>>(input => input.Select(b => new BlockDto
            {
                Height = b.Height
            }));

        // Act
        var dto = await _handler.Handle(request, CancellationToken.None);

        // Assert
        AssertNext(dto.Cursor, blocks[^1].Height);
        dto.Cursor.Previous.Should().Be(null);
    }

    private static void AssertNext(CursorDto dto, ulong pointer)
    {
        BlocksCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Forward);
        next.Pointer.Should().Be(pointer);
    }

    private static void AssertPrevious(CursorDto dto, ulong pointer)
    {
        BlocksCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
        next.PagingDirection.Should().Be(PagingDirection.Backward);
        next.Pointer.Should().Be(pointer);
    }
}
