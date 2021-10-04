using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks
{
    public class RetrieveCirrusBestBlockReceiptQueryHandlerTests
    {
        private readonly RetrieveCirrusBestBlockReceiptQueryHandler _handler;
        private readonly Mock<IMediator> _mediator;

        public RetrieveCirrusBestBlockReceiptQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveCirrusBestBlockReceiptQueryHandler(_mediator.Object);
        }

        [Fact]
        public async Task RetrieveCirrusBestBlockReceiptQuery_Sends_CallCirrusGetBestBlockReceiptQuery()
        {
            // Arrange
            // Act
            await _handler.Handle(new RetrieveCirrusBestBlockReceiptQuery(), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
