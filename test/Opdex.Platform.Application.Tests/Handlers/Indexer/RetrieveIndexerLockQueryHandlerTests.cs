using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Indexer
{
    public class RetrieveIndexerLockQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveIndexerLockQueryHandler _handler;

        public RetrieveIndexerLockQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveIndexerLockQueryHandler(_mediator.Object);
        }

        [Fact]
        public async Task Send_SelectIndexerUnlockCommand()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new RetrieveIndexerLockQuery(), token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<SelectIndexerLockQuery>(), token), Times.Once);
        }
    }
}
