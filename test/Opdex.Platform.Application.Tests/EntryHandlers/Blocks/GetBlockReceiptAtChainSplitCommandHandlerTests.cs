using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks;

public class GetBlockReceiptAtChainSplitCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetBlockReceiptAtChainSplitCommandHandler _handler;

    public GetBlockReceiptAtChainSplitCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new GetBlockReceiptAtChainSplitCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveLatestBlockQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _handler.Handle(new GetBlockReceiptAtChainSplitCommand(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLatestBlockQuery>(q => q.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveBlockByHeightQuery_SendReorgCount()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedCallCount = 10;
        var reorgCount = expectedCallCount;

        var block = new Block(50000, new Sha256(UInt256.Parse("423498324932")), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(block);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), CancellationToken.None))
                     .Callback((() => reorgCount--))
                     .ReturnsAsync(() => reorgCount == 0 ? blockReceipt : null);

        // Act
        await _handler.Handle(new GetBlockReceiptAtChainSplitCommand(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveBlockByHeightQuery>(q => q.FindOrThrow), cancellationToken), Times.Exactly(expectedCallCount));
    }

    [Fact]
    public async Task Handle_RetrieveCirrusBlockReceiptByHashQuery_SendReorgCount()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedCallCount = 10;
        var reorgCount = expectedCallCount;

        var block = new Block(50000, new Sha256(UInt256.Parse("423498324932")), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(block);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), CancellationToken.None))
            .Callback((() => reorgCount--))
            .ReturnsAsync(() => reorgCount == 0 ? blockReceipt : null);

        // Act
        await _handler.Handle(new GetBlockReceiptAtChainSplitCommand(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveCirrusBlockReceiptByHashQuery>(q => q.Hash == block.Hash && !q.FindOrThrow), cancellationToken), Times.Exactly(expectedCallCount));
    }

    [Fact]
    public async Task Handle_ExceedsMaxReorgLimit_ThrowMaxReorgException()
    {
        var block = new Block(50000, new Sha256(UInt256.Parse("423498324932")), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(block);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), CancellationToken.None))
                     .ReturnsAsync((BlockReceipt)null);

        // Act
        Task Act() => _handler.Handle(new GetBlockReceiptAtChainSplitCommand(), CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MaximumReorgException>(Act);
    }

    [Fact]
    public async Task Handle_LessThanMaxReorg_ReturnCirrusBlockReceipt()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedCallCount = 5;
        var reorgCount = expectedCallCount;

        var block = new Block(50000, new Sha256(UInt256.Parse("423498324932")), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(block);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 20, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), CancellationToken.None))
            .Callback((() => reorgCount--))
            .ReturnsAsync(() => reorgCount == 0 ? blockReceipt : null);

        // Act
        var result = await _handler.Handle(new GetBlockReceiptAtChainSplitCommand(), cancellationToken);

        // Assert
        result.Should().Be(blockReceipt);
    }
}
