using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks
{
    public class RetrieveCirrusBlockReceiptByHashQueryHandlerTests
    {
        private readonly RetrieveCirrusBlockReceiptByHashQueryHandler _handler;
        private readonly Mock<IMediator> _mediator;

        public RetrieveCirrusBlockReceiptByHashQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveCirrusBlockReceiptByHashQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveCirrusBlockReceiptByHashQuery_InvalidHash_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            static void Act() => new RetrieveCirrusBlockReceiptByHashQuery(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Block hash must be provided.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RetrieveCirrusBlockReceiptByHashQuery_Sends_CallCirrusGetBlockReceiptByHashQuery(bool findOrThrow)
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";

            // Act
            await _handler.Handle(new RetrieveCirrusBlockReceiptByHashQuery(hash, findOrThrow), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetBlockReceiptByHashQuery>(q => q.Hash == hash &&
                                                                                                    q.FindOrThrow == findOrThrow),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
