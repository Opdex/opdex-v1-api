using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks
{
    public class MakeIndexerUnlockCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeIndexerUnlockCommandHandler _handler;

        public MakeIndexerUnlockCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeIndexerUnlockCommandHandler(_mediator.Object, new NullLogger<MakeIndexerUnlockCommandHandler>());
        }

        [Fact]
        public async Task Send_PersistIndexerUnlockCommand()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new MakeIndexerUnlockCommand(), token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerUnlockCommand>(), token), Times.Once);
        }

        [Fact]
        public async Task UnableToUnlockIndexer_ThrowException()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            Task Act() => _handler.Handle(new MakeIndexerUnlockCommand(), token);

            // Assert
            await Assert.ThrowsAsync<Exception>(Act);
        }
    }
}