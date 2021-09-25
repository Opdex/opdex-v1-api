using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Markets
{
    public class RetrieveMarketsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveMarketsByModifiedBlockQueryHandler _handler;

        public RetrieveMarketsByModifiedBlockQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveMarketsByModifiedBlockQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveMarketsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveMarketsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task RetrieveMarketsByModifiedBlockQuery_Sends_SelectMarketByModifiedBlockQuery()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveMarketsByModifiedBlockQuery(blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectMarketsByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
