using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks
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
            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new MakeIndexerLockCommand(), token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), token), Times.Once);
        }

        [Fact]
        public async Task CannotPersistIndexerLock_ThrowIndexerAlreadyRunningException()
        {
            // Arrange
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            // Act
            Task Act() => _handler.Handle(new MakeIndexerLockCommand(), default);

            // Assert
            await Assert.ThrowsAsync<IndexingAlreadyRunningException>(Act);
        }

        [Fact]
        public async Task CanPersistIndexerLock_DoNotThrow()
        {
            // Arrange
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act
            Task Act() => _handler.Handle(new MakeIndexerLockCommand(), default);
            var exception = await Record.ExceptionAsync(Act);

            // Assert
            exception.Should().BeNull();
        }
    }
}