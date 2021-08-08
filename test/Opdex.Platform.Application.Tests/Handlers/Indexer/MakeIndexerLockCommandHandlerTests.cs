using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Indexer
{
    public class MakeIndexerLockCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeIndexerLockCommandHandler _handler;

        public MakeIndexerLockCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeIndexerLockCommandHandler(_mediator.Object, new NullLogger<MakeIndexerLockCommandHandler>());
        }

        [Fact]
        public async Task Send_PersistIndexerLockCommand()
        {
            // Arrange
            var token = CancellationToken.None;

            // Act
            try
            {
                await _handler.Handle(new MakeIndexerLockCommand(), token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task CannotPersistIndexerLock_ThrowIndexerAlreadyRunningException()
        {
            // Arrange
            var token = CancellationToken.None;
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            // Act
            Task Act() => _handler.Handle(new MakeIndexerLockCommand(), token);

            // Assert
            await Assert.ThrowsAsync<IndexingAlreadyRunningException>(Act);
        }

        [Fact]
        public async Task CanPersistIndexerLock_DoNotThrow()
        {
            // Arrange
            var token = CancellationToken.None;
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act
            Task Act() => _handler.Handle(new MakeIndexerLockCommand(), token);
            var exception = await Record.ExceptionAsync(Act);

            // Assert
            exception.Should().BeNull();
        }
    }
}
