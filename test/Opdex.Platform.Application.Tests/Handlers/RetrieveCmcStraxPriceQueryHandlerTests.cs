using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers
{
    public class RetrieveCmcStraxPriceQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<ILogger<RetrieveCmcStraxPriceQueryHandler>> _logger;
        private readonly RetrieveCmcStraxPriceQueryHandler _handler;

        public RetrieveCmcStraxPriceQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RetrieveCmcStraxPriceQueryHandler>>();
            _handler = new RetrieveCmcStraxPriceQueryHandler(_mediator.Object, _logger.Object);
        }

        [Fact]
        public void RetrieveCmcStraxPriceQuery_InvalidBlockTime_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveCmcStraxPriceQuery(default);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block time must be set.");
        }

        [Fact]
        public async Task RetrieveCmcStraxPriceQuery_Sends_CallCmcGetStraxLatestQuoteQuery()
        {
            // Arrange
            var blocktime = DateTime.UtcNow;

            // Act
            try
            {
                await _handler.Handle(new RetrieveCmcStraxPriceQuery(blocktime), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxLatestQuoteQuery>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task RetrieveCmcStraxPriceQuery_Sends_CallCmcGetStraxHistoricalQuoteQuery()
        {
            // Arrange
            var blocktime = DateTime.UtcNow.AddMinutes(-5);

            // Act
            try
            {
                await _handler.Handle(new RetrieveCmcStraxPriceQuery(blocktime), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCmcGetStraxHistoricalQuoteQuery>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task RetrieveCmcStraxPriceQuery_Error_CatchAndReturn()
        {
            // Arrange
            var blocktime = DateTime.UtcNow.AddMinutes(-5);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCmcGetStraxHistoricalQuoteQuery>(), CancellationToken.None))
                .Throws<Exception>();

            // Act
            var response = await _handler.Handle(new RetrieveCmcStraxPriceQuery(blocktime), CancellationToken.None);

            // Assert
            response.Should().Be(0m);
        }
    }
}
