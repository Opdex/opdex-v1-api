using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests
{
    public class IndexerBackgroundServiceTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly IndexerBackgroundService _indexerService;
        private readonly string Identity;

        public IndexerBackgroundServiceTests()
        {
            _mediator = new Mock<IMediator>();

            var opdexConfiguration = new OpdexConfiguration {Network = NetworkType.DEVNET};
            Identity = opdexConfiguration.InstanceId;

            var logger = new NullLogger<IndexerBackgroundService>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(provider => _mediator.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _indexerService = new IndexerBackgroundService(serviceProvider, opdexConfiguration, logger);
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
            var indexLock = new IndexLock(false, false, Identity, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

            // Act
            await _indexerService.StartAsync(token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task ExecuteAsync_RetrievesIndexLock_IndexingAlreadyRunning()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;
            var indexLock = new IndexLock(true, true, Identity, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

            // Act
            await _indexerService.StartAsync(token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never());
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task ExecuteAsync_RetrievesIndexLock_ThrowsIndexingAlreadyRunningException()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;
            var indexLock = new IndexLock(true, false, Identity, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                .Throws<IndexingAlreadyRunningException>();

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
            var indexLock = new IndexLock(true, false, Identity, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

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
            var indexLock = new IndexLock(true, false, Identity, DateTime.UtcNow);

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
            var newIdentity = Guid.NewGuid().ToString();
            var token = new CancellationTokenSource().Token;
            var indexLock = new IndexLock(true, true, newIdentity, DateTime.UtcNow);

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
            var indexLock = new IndexLock(true, true, Identity, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveIndexerLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(indexLock);

            // Act
            await _indexerService.StopAsync(token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
