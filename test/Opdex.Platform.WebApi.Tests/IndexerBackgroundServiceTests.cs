using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests;

public class IndexerBackgroundServiceTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IOptionsMonitor<IndexerConfiguration>> _indexerOptionsMonitorMock;
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

        _indexerOptionsMonitorMock = new Mock<IOptionsMonitor<IndexerConfiguration>>();

        _indexerService = new IndexerBackgroundService(scopeFactoryMock.Object, opdexConfiguration, new NullLogger<IndexerBackgroundService>(), _indexerOptionsMonitorMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_RetrievesIndexLock_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_RetrievesIndexLock_IndexingUnavailable()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(false, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_IndexerStoppedFromConfig_IndexingUnavailable()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = false });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();
        // _primaryIdentity trying to index when _otherIdentity already is
        var indexLock = new IndexLock(true, true, _otherIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Rewinding, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Deploying, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var blockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        var bestBlock = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(bestBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        var bestBlock = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(bestBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_BestBlockReceiptNotNull_PerformIndexing()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        var bestBlock = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(bestBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeIndexerLockCommand>(q => q.Reason == IndexLockReason.Indexing), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        _mediator.Verify(callTo => callTo.Send(It.Is<ProcessLatestBlocksCommand>(c => c.CurrentBlock == bestBlock), It.IsAny<CancellationToken>()), Times.Once());
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    // Todo: Need to create a mockable service to implement Task.Delay
    // -- Task.Delay is complicated testing, work around by implementing a service and mocking in tests.
    // -- Waiting for 3 failures from GetBestBlockReceiptQuery requires the above fix for tests to be run
    // [Fact]
    // public async Task ExecuteAsync_BestBlockReceiptNull_PerformReindexing()
    // {
    //     // Arrange
    //     using var cancellationTokenSource = new CancellationTokenSource();
    //     var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);
    //
    //     _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((BlockReceipt)null);
    //     _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
    //     _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
    //     var chainSplitBlockReceipt = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
    //     _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBlockReceiptAtChainSplitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(chainSplitBlockReceipt);
    //     _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
    //
    //     _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });
    //
    //     // Act
    //     await _indexerService.StartAsync(cancellationTokenSource.Token);
    //
    //     // Assert
    //     _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    //     _mediator.Verify(callTo => callTo.Send(It.Is<MakeIndexerLockCommand>(q => q.Reason == IndexLockReason.Rewinding), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    //     _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindToBlockCommand>(c => c.Block == chainSplitBlockReceipt.Height), It.IsAny<CancellationToken>()), Times.Once());
    //     _mediator.Verify(callTo => callTo.Send(It.Is<ProcessLatestBlocksCommand>(c => c.CurrentBlock == chainSplitBlockReceipt), It.IsAny<CancellationToken>()), Times.Once());
    //     _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    // }

    [Fact]
    public async Task ExecuteAsync_HappyPath()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, false, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);


        var bestBlock = new BlockReceipt(new Sha256(5340958239), 1, DateTime.UtcNow, DateTime.UtcNow, new Sha256(3343544543), new Sha256(34325), new Sha256(13249049), Array.Empty<Sha256>());
        _mediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(bestBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _indexerOptionsMonitorMock.Setup(callTo => callTo.CurrentValue).Returns(new IndexerConfiguration { Enabled = true });

        // Act
        await _indexerService.StartAsync(cancellationTokenSource.Token);

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
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        await _indexerService.StopAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task StopAsync_DoesNotSend_MakeIndexerUnlockCommand_NotLocked()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, false, _primaryIdentity, default, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StopAsync_DoesNotSend_MakeIndexerUnlockCommand_DifferentInstance()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, true, _otherIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StopAsync_Sends_MakeIndexerUnlockCommand()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var indexLock = new IndexLock(true, true, _primaryIdentity, IndexLockReason.Indexing, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

        // Act
        await _indexerService.StopAsync(cancellationTokenSource.Token);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
