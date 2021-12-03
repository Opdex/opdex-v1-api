using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools;

public class CreateMiningPoolCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateMiningPoolCommandHandler _handler;

    public CreateMiningPoolCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateMiningPoolCommandHandler(_mediator.Object);
    }

    [Fact]
    public void CreateMiningPoolCommand_InvalidLiquidityPoolAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address liquidityPoolAddress = Address.Empty;
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Liquidity pool address must be provided.");
    }

    [Fact]
    public void CreateMiningPoolCommand_InvalidLiquidityPoolId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 0;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Liquidity pool id must be greater than zero.");
    }

    [Fact]
    public void CreateMiningPoolCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CreateMiningPoolCommand_Sends_CallCirrusGetSmartContractPropertyQuery()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.BlockHeight == blockHeight &&
                                                                                                   q.Contract == liquidityPoolAddress &&
                                                                                                   q.PropertyType == SmartContractParameterType.Address &&
                                                                                                   q.PropertyStateKey == StakingPoolConstants.StateKeys.MiningPool),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMiningPoolCommand_Sends_RetrieveMiningPoolByAddressQuery()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 10;

        Address miningPoolAddress = "PNG7FiEUa89Xh2WU8q87nq2KGFTtoSPBDE";

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSmartContractPropertyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmartContractMethodParameter(miningPoolAddress));

        // Act
        try
        {
            await _handler.Handle(new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight), CancellationToken.None);
        }
        catch { }


        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByAddressQuery>(q => q.Address == miningPoolAddress &&
                                                                                            q.FindOrThrow == false),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMiningPoolCommand_Returns_MiningPoolAlreadyExists()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 10;

        Address miningPoolAddress = "PNG7FiEUa89Xh2WU8q87nq2KGFTtoSPBDE";
        const ulong miningPoolId = 2;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSmartContractPropertyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmartContractMethodParameter(miningPoolAddress));

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MiningPool(miningPoolId, liquidityPoolId, miningPoolAddress, 3, 4, 5, 6, 7));

        // Act
        var response = await _handler.Handle(new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight), CancellationToken.None);

        // Assert
        response.Should().Be(miningPoolId);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeMiningPoolCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateMiningPoolCommand_Sends_MakeMiningPoolCommand()
    {
        // Arrange
        Address liquidityPoolAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong liquidityPoolId = 1;
        const ulong blockHeight = 10;

        Address miningPoolAddress = "PNG7FiEUa89Xh2WU8q87nq2KGFTtoSPBDE";

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSmartContractPropertyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmartContractMethodParameter(miningPoolAddress));

        // Act
        await _handler.Handle(new CreateMiningPoolCommand(liquidityPoolAddress, liquidityPoolId, blockHeight), CancellationToken.None);


        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningPoolCommand>(q => q.MiningPool.Address == miningPoolAddress &&
                                                                                 q.MiningPool.Id == 0 &&
                                                                                 q.MiningPool.LiquidityPoolId == liquidityPoolId &&
                                                                                 q.BlockHeight == blockHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}