using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets
{
    public class CreateRewindMarketsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindMarketsCommandHandler _handler;

        public CreateRewindMarketsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindMarketsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindMarketsCommandHandler>>());
        }

        [Fact]
        public void CreateRewindMarketsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindMarketsCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindMarketsCommand_Sends_RetrieveMarketsByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMarketsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindMarketsCommand_Sends_MakeMarketCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var markets = new List<Market>
            {
                new Market(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 3, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", false, false, false, 3, true, 10, 11),
                new Market(1, "PvLgqqARwmH1iT1GLsMroh6zXXNMU9Ejmi", 2, 0, "PRwmH1iT1GLsMroh6zXXMU9EjmivLgqqAN", true, true, true, 3, true, 10, 11)
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMarketsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(markets);

            // Act
            await _handler.Handle(new CreateRewindMarketsCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var market in markets)
            {
                var times = market.IsStakingMarket ? Times.Never() : Times.Once();

                _mediator.Verify(callTo => callTo.Send(It.Is<MakeMarketCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                     q.Market == market &&
                                                                                     q.RefreshOwner == true),
                                                       It.IsAny<CancellationToken>()), times);
            }
        }
    }
}
