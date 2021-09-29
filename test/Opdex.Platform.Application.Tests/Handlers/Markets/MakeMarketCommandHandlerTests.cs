using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Markets
{
    public class MakeMarketCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeMarketCommandHandler _handler;

        public MakeMarketCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeMarketCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeMarketCommand_InvalidMarket_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeMarketCommand(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Market must be provided.");
        }

        [Fact]
        public void MakeMarketCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var market = new Market(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 3, Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true,
                                        false, false, 3, true, 10, 11);

            // Act
            void Act() => new MakeMarketCommand(market, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, true)]
        public async Task MakeMarketCommand_Sends_RetrieveMarketContractSummaryByAddressQuery(bool refreshPendingOwner,
                                                                                              bool refreshOwner,
                                                                                              bool expected)
        {
            // Arrange
            const ulong blockHeight = 20;
            var stakingTokenId = refreshOwner || refreshPendingOwner ? 0 : 1;
            var market = new Market(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, stakingTokenId, Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true,
                                        false, false, 3, true, 10, 11);

            // Act
            try
            {
                await _handler.Handle(new MakeMarketCommand(market, blockHeight,
                                                            refreshPendingOwner: refreshPendingOwner,
                                                            refreshOwner: refreshOwner), CancellationToken.None);
            }
            catch { }

            // Assert
            var times = expected ? Times.Once() : Times.Never();

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketContractSummaryQuery>(q => q.Market == market.Address &&
                                                                                                  q.BlockHeight == blockHeight &&
                                                                                                  q.IncludePendingOwner == refreshPendingOwner &&
                                                                                                  q.IncludeOwner == refreshOwner),
                                                   It.IsAny<CancellationToken>()), times);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task MakeMarketCommand_Sends_PersistMarketCommand(bool refreshPendingOwner,
                                                                       bool refreshOwner)
        {
            // Arrange
            const ulong blockHeight = 20;
            var stakingTokenId = refreshOwner ? 0 : 1;

            Address currentPendingOwner = Address.Empty;
            Address updatedPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

            Address currentOwner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
            Address updatedOwner = refreshOwner ? "PRwmH1iT1GLsMroh6zXXNMU9EjmivLgqqA" : currentOwner;

            var market = new Market(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, stakingTokenId, Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true,
                                    false, false, 3, true, 10, 11);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var summary = new MarketContractSummary(blockHeight);

                    if (refreshPendingOwner) summary.SetPendingOwner(new SmartContractMethodParameter(updatedPendingOwner));
                    if (refreshOwner) summary.SetOwner(new SmartContractMethodParameter(updatedOwner));

                    return summary;
                });

            // Act
            try
            {
                await _handler.Handle(new MakeMarketCommand(market, blockHeight,
                                                            refreshPendingOwner: refreshPendingOwner,
                                                            refreshOwner: refreshOwner), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistMarketCommand>(q => q.Market == market),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
