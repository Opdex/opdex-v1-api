using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Index;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.IndexControllerTests;

public class RewindTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly IndexerController _controller;
    private readonly NetworkType _network = NetworkType.DEVNET;

    public RewindTests()
    {
        _mediator = new Mock<IMediator>();

        var opdexConfiguration = new OpdexConfiguration { Network = _network };

        _controller = new IndexerController(Mock.Of<IMapper>(), _mediator.Object, opdexConfiguration);
    }

    [Fact]
    public async Task Rewind_Sends_MakeIndexerLockCommand()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Block(5000000000000, new Sha256(4234238947328), DateTime.UtcNow, DateTime.UtcNow));

        // Act
        try
        {
            await _controller.Rewind(request, cancellationTokenSource.Token);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Rewind_IndexLockReturnsFalse_ThrowIndexingAlreadyRunningException()
    {
        // Arrange
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Block(5000000000000, new Sha256(4234238947328), DateTime.UtcNow, DateTime.UtcNow));
        _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        Task Act() => _controller.Rewind(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<IndexingAlreadyRunningException>(Act);
    }

    [Fact]
    public async Task Rewind_CreateRewindToBlockCommand_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Block(50000000000, new Sha256(4234238947328), DateTime.UtcNow, DateTime.UtcNow));
        _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockHashByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Sha256(235983290483209));
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);

        // Act
        try
        {
            var response = await _controller.Rewind(request, cancellationTokenSource.Token);
        }
        catch (Exception)
        {
            // ignore
        }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindToBlockCommand>(q => q.Block == request.Block), CancellationToken.None), Times.Once);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Rewind_RewindToBlockReturnsFalse_ThrowException()
    {
        // Arrange
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Block(50000000000, new Sha256(4234238947328), DateTime.UtcNow, DateTime.UtcNow));
        _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockHashByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Sha256(235983290483209));
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        Task Act() => _controller.Rewind(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<Exception>(Act);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Rewind_ProcessLatestBlocks_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Block(request.Block, new Sha256(5340958239), DateTime.UtcNow, DateTime.UtcNow));
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _controller.Rewind(request, cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<ProcessLatestBlocksCommand>(c => c.CurrentBlock == blockReceipt && c.NetworkType == _network), CancellationToken.None), Times.Once);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Rewind_ProcessLatestBlocksFinished_Return204()
    {
        // Arrange;
        var request = new RewindRequest { Block = 10 };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Block(50000000000, new Sha256(4234238947328), DateTime.UtcNow, DateTime.UtcNow));
        _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Block(request.Block, new Sha256(5340958239), DateTime.UtcNow, DateTime.UtcNow));
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBlockReceiptByHashQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var response = await _controller.Rewind(request, CancellationToken.None);

        // Assert
        response.As<StatusCodeResult>().StatusCode.Should().Be(StatusCodes.Status204NoContent);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
    }
}
