using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Tokens
{
    public class RetrieveTokenByIdQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveTokenByIdQueryHandler _handler;

        public RetrieveTokenByIdQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveTokenByIdQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveTokenByIdQuery_InvalidAddress_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong tokenId = 0ul;

            // Act
            void Act() => new RetrieveTokenByIdQuery(tokenId);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("TokenId must be greater than 0.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RetrieveTokenByIdQuery_Sends_SelectTokenByIdQuery(bool findOrThrow)
        {
            // Arrange
            const ulong tokenId = 10ul;

            // Act
            await _handler.Handle(new RetrieveTokenByIdQuery(tokenId, findOrThrow), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectTokenByIdQuery>(query => query.TokenId == tokenId &&
                                                                                        query.FindOrThrow == findOrThrow),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
