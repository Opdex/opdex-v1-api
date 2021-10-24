using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Tokens
{
    public class RetrieveTokenSummaryByMarketAndTokenIdQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveTokenSummaryByMarketAndTokenIdQueryHandler _handler;

        public RetrieveTokenSummaryByMarketAndTokenIdQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveTokenSummaryByMarketAndTokenIdQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveTokenSummaryByMarketAndTokenIdQuery_InvalidAddress_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong tokenId = 0ul;
            const ulong marketId = 0ul;

            // Act
            void Act() => new RetrieveTokenSummaryByMarketAndTokenIdQuery(marketId, tokenId);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("TokenId must be greater than 0.");
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public async Task RetrieveTokenSummaryByMarketAndTokenIdQuery_Sends_SelectTokenByIdQuery(ulong marketId, bool findOrThrow)
        {
            // Arrange
            const ulong tokenId = 10ul;

            // Act
            await _handler.Handle(new RetrieveTokenSummaryByMarketAndTokenIdQuery(marketId, tokenId, findOrThrow), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectTokenSummaryByMarketAndTokenIdQuery>(query => query.TokenId == tokenId &&
                                                                                                             query.MarketId == marketId &&
                                                                                                             query.FindOrThrow == findOrThrow),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
