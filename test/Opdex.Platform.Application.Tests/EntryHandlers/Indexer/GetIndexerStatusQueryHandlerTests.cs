using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Index;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Application.EntryHandlers.Indexer;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Indexer;

public class GetIndexerStatusQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetIndexerStatusQueryHandler _handler;

    public GetIndexerStatusQueryHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new GetIndexerStatusQueryHandler(_mapperMock.Object, _mediatorMock.Object);
    }

    public async Task Handle_RetrieveIndexerLockQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(new GetIndexerStatusQuery(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), cancellationToken), Times.Once);
    }

    public async Task Handle_RetrieveLatestBlockQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(new GetIndexerStatusQuery(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLatestBlockQuery>(q => !q.FindOrThrow), cancellationToken), Times.Once);
    }

    public async Task Handle_Map_Results()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = new CancellationToken();

        var indexLock = new IndexLock(true, true, Guid.NewGuid().ToString(), IndexLockReason.Indexing, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), CancellationToken.None))
            .ReturnsAsync(indexLock);

        var block = new Block(50000, new Sha256(UInt256.Parse("429340234238923904")), DateTime.UtcNow, DateTime.UtcNow.AddSeconds(-10));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None))
            .ReturnsAsync(block);

        // Act
        await _handler.Handle(new GetIndexerStatusQuery(), cancellationToken);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<BlockDto>(block), Times.Once);
        _mapperMock.Verify(callTo => callTo.Map<IndexerStatusDto>(indexLock), Times.Once);
    }

    public async Task Handle_Map_ReturnLatestBlock()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = new CancellationToken();

        var indexLock = new IndexLock(true, true, Guid.NewGuid().ToString(), IndexLockReason.Indexing, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), CancellationToken.None))
            .ReturnsAsync(indexLock);

        var block = new Block(50000, new Sha256(UInt256.Parse("429340234238923904")), DateTime.UtcNow, DateTime.UtcNow.AddSeconds(-10));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None))
            .ReturnsAsync(block);

        var blockDto = new BlockDto();
        _mapperMock.Setup(callTo => callTo.Map<BlockDto>(It.IsAny<Block>())).Returns(blockDto);
        var indexerStatusDto = new IndexerStatusDto();
        _mapperMock.Setup(callTo => callTo.Map<IndexerStatusDto>(It.IsAny<IndexLock>())).Returns(indexerStatusDto);

        // Act
        var response = await _handler.Handle(new GetIndexerStatusQuery(), cancellationToken);

        // Assert
        response.LatestBlock.Should().Be(blockDto);
    }
}
