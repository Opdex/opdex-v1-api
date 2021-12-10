using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests;

public class IndexerBackgroundServiceTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly IndexerBackgroundService _indexerService;
    private readonly string _primaryIdentity;
    private readonly string _otherIdentity;

    public IndexerBackgroundServiceTests()
    {
        _mediator = new Mock<IMediator>();

        var opdexConfiguration = new OpdexConfiguration { Network = NetworkType.DEVNET };
        _primaryIdentity = opdexConfiguration.InstanceId;
        _otherIdentity = Guid.NewGuid().ToString();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(provider => _mediator.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(callTo => callTo.ServiceProvider).Returns(serviceProvider);
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(callTo => callTo.CreateScope()).Returns(new AsyncServiceScope(serviceScopeMock.Object));

        _indexerService = new IndexerBackgroundService(scopeFactoryMock.Object, opdexConfiguration, new NullLogger<IndexerBackgroundService>());
    }

    [Fact]
    public async Task ExecuteAsync_RetrievesIndexLock_Send()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_RetrievesIndexLock_IndexingUnavailable()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(false, false, _primaryIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_AnotherInstanceLockedIndexer_DoNotIndexOrUnlock()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        // _primaryIdentity trying to index when _otherIdentity already is
        var indexLock = new IndexLock(true, true, _otherIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_SameInstanceLockedIndexerForRewind_DoNotIndexOrUnlock()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Rewind, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_SameInstanceLockedIndexerForDeployment_DoNotIndexOrUnlock()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Deploy, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_SameInstanceLockedIndexerForIndexing_ForceUnlockAndReprocess()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_LockingIndexerFails_DoNotProcessBlocksOrUnlock()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_HappyPath()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _indexerService.StartAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task StopAsync_Sends_RetrieveIndexerLockQuery()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;

        // Act
        await _indexerService.StopAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task StopAsync_DoesNotSend_MakeIndexerUnlockCommand_NotLocked()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, false, _primaryIdentity, default, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StopAsync_DoesNotSend_MakeIndexerUnlockCommand_DifferentInstance()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, true, _otherIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StopAsync_Sends_MakeIndexerUnlockCommand()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Index, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
