using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.EntryHandlers.Addresses.Mining;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Mining
{
    public class CreateRewindMiningPositionsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindMiningPositionsCommandHandler _handler;

        public CreateRewindMiningPositionsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindMiningPositionsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindMiningPositionsCommandHandler>>());
        }

        [Fact]
        public async Task Handle_Sends_RetrieveMiningPositionsByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMiningPositionsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPositionsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Sends_RetrieveMiningPoolByIdQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;
            var miningPosition = new AddressMining(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 1000, 20, 50);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPositionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<AddressMining> { miningPosition });

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMiningPositionsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByIdQuery>(q => q.MiningPoolId == miningPosition.MiningPoolId),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Sends_MakeAddressMiningCommand()
        {
            // Arrange
            const ulong rewindHeight = 1000000000000;
            var miningPositions = new List<AddressMining>
            {
                new AddressMining(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 1000, 20, 50),
                new AddressMining(2, 2, "P5uJYUcmAsqAEgUXjBJPuCXfcNKdN28FQf", 500, 20, 50)
            };

            var miningPool = new MiningPool(2, 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", 10000000000, 5000, 288282822, 4, 5);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPositionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(miningPositions);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByIdQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(miningPool);

            UInt256 updatedBalance = 5000000;
            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetMiningBalanceForAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedBalance);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMiningPositionsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            foreach (var miningPosition in miningPositions)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeAddressMiningCommand>(q => ReferenceEquals(miningPosition, q.AddressMining)
                                                                                         && q.AddressMining.Balance == updatedBalance
                                                                                         && q.AddressMining.ModifiedBlock == rewindHeight),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
