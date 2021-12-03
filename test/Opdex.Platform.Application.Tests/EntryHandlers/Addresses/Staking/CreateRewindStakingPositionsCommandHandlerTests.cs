using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.EntryHandlers.Addresses.Staking;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Staking;

public class CreateRewindStakingPositionsCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindStakingPositionsCommandHandler _handler;

    public CreateRewindStakingPositionsCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindStakingPositionsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindStakingPositionsCommandHandler>>());
    }

    [Fact]
    public async Task Handle_Sends_RetrieveStakingPositionsByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindStakingPositionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveStakingPositionsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Sends_RetrieveLiquidityPoolByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        var stakingPosition = new AddressStaking(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 1000, 20, 50);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AddressStaking> { stakingPosition });

        // Act
        try
        {
            await _handler.Handle(new CreateRewindStakingPositionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByIdQuery>(q => q.LiquidityPoolId == stakingPosition.LiquidityPoolId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Sends_MakeAddressStakingCommand()
    {
        // Arrange
        const ulong rewindHeight = 1000000000000;
        var stakingPositions = new List<AddressStaking>
        {
            new AddressStaking(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 1000, 20, 50),
            new AddressStaking(2, 2, "P5uJYUcmAsqAEgUXjBJPuCXfcNKdN28FQf", 500, 20, 50)
        };

        var liquidityPool = new LiquidityPool(2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "ETH-CRS", 8, 7, 6, 4, 5);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stakingPositions);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(liquidityPool);

        UInt256 updatedWeight = 5000000;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStakingWeightForAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedWeight);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindStakingPositionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var stakingPosition in stakingPositions)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeAddressStakingCommand>(q => ReferenceEquals(stakingPosition, q.AddressStaking)
                                                                                         && q.AddressStaking.Weight == updatedWeight
                                                                                         && q.AddressStaking.ModifiedBlock == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}