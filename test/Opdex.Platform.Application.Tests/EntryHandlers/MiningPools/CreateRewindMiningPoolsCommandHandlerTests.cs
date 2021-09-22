using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Domain.Models.MiningPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools
{
    public class CreateRewindMiningPoolsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindMiningPoolsCommandHandler _handler;

        public CreateRewindMiningPoolsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindMiningPoolsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindMiningPoolsCommandHandler>>());
        }

        [Fact]
        public void CreateRewindMiningPoolsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindMiningPoolsCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindMiningPoolsCommand_Sends_RetrieveMiningPoolsByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMiningPoolsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindMiningPoolsCommand_Sends_MakeMiningPoolCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var miningPools = new List<MiningPool>
            {
                new MiningPool(1, 2, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 3, 5, 5, 6, 7),
                new MiningPool(2, 2, "jmivLgqqARwmH1iPT1GLsMroh6zXXNMU9E", 4, 6, 6, 7, 8),
                new MiningPool(3, 2, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 5, 7, 7, 8, 9),
                new MiningPool(4, 2, "Poh6zXXNMU9EjmivLgqqARwmT1GLsMrH1i", 6, 8, 8, 9, 10),
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningPoolsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);

            // Act
            await _handler.Handle(new CreateRewindMiningPoolsCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var pool in miningPools)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningPoolCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                         q.MiningPool == pool &&
                                                                                         q.RefreshRewardPerBlock == true &&
                                                                                         q.RefreshRewardPerLpt == true &&
                                                                                         q.RefreshMiningPeriodEndBlock == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
