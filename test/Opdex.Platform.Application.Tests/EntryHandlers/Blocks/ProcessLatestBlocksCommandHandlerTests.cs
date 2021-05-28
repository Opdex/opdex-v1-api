using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks
{
    public class ProcessLatestBlocksCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly ProcessLatestBlocksCommandHandler _processLatestBlocksCommandHandler;

        public ProcessLatestBlocksCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _processLatestBlocksCommandHandler = new ProcessLatestBlocksCommandHandler(_mediator.Object,
                                                                                       new NullLogger<ProcessLatestBlocksCommandHandler>());
        }

        [Fact]
        public async Task Send_PersistIndexerLockCommand()
        {
            // Arrange
            var request = new ProcessLatestBlocksCommand(false);
            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _processLatestBlocksCommandHandler.Handle(request, token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), token), Times.Once);
        }

        [Fact]
        public async Task PersistIndexerLockCommand_ReturnsFalse_ThrowIndexerAlreadyRunningException()
        {
            // Arrange
            var request = new ProcessLatestBlocksCommand(false);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            // Act
            Task Act() => _processLatestBlocksCommandHandler.Handle(request, default);

            // Assert
            await Assert.ThrowsAsync<IndexingAlreadyRunningException>(Act);
        }

        [Fact]
        public async Task PersistIndexerLockCommand_ReturnsFalse_SendIndexLatestBlocksCommand()
        {
            // Arrange
            var request = new ProcessLatestBlocksCommand(false);
            var token = new CancellationTokenSource().Token;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act
            await _processLatestBlocksCommandHandler.Handle(request, token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<IndexLatestBlocksCommand>(), token), Times.Once);
        }

        [Fact]
        public async Task Always_Send_PersistIndexerUnlockCommand()
        {
            // Arrange
            var request = new ProcessLatestBlocksCommand(false);
            var token = new CancellationTokenSource().Token;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<IndexLatestBlocksCommand>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            try
            {
                await _processLatestBlocksCommandHandler.Handle(request, token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
        }
    }
}