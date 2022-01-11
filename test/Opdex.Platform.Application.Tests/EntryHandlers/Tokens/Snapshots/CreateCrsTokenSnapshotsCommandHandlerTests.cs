using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens.Snapshots;

public class CreateCrsTokenSnapshotsCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<CreateCrsTokenSnapshotsCommandHandler>> _logger;
    private readonly CreateCrsTokenSnapshotsCommandHandler _handler;

    public CreateCrsTokenSnapshotsCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<CreateCrsTokenSnapshotsCommandHandler>>();
        _handler = new CreateCrsTokenSnapshotsCommandHandler(_mediator.Object, _logger.Object);
    }

    [Fact]
    public void CreateCrsTokenSnapshotsCommand_InvalidBlockTime_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateCrsTokenSnapshotsCommand(default, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block time must be set.");
    }

    [Fact]
    public void CreateCrsTokenSnapshotsCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveTokenByAddressQuery()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == Address.Cirrus), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_MakeTokenCommand()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenCommand>(q => q.Token.Address == Address.Cirrus &&
                                                                            q.Token.IsLpt == false &&
                                                                            q.Token.Symbol == TokenConstants.Cirrus.Symbol &&
                                                                            q.Token.Name == TokenConstants.Cirrus.Name &&
                                                                            q.Token.Decimals == TokenConstants.Cirrus.Decimals &&
                                                                            q.Token.TotalSupply == TokenConstants.Cirrus.TotalSupply &&
                                                                            q.BlockHeight == blockHeight), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_InvalidCrsId_ThrowsException()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 10;

        // Act
        // Assert
        await _handler
            .Invoking(h => h.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None))
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("Unable to create CRS token during create snapshot.");
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveTokenSnapshotWithFilterQuery()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                              TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        // Act
        try
        {
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == token.Id &&
                                                                                                q.MarketId == 0 &&
                                                                                                q.DateTime == blockTime &&
                                                                                                q.SnapshotType == SnapshotType.Minute), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_ReturnsTrue_SnapshotExists()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                              TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);
        var latestSnapshot = new TokenSnapshot(1, 2, 3, new Ohlc<decimal>(), SnapshotType.Daily, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
            .ReturnsAsync(latestSnapshot);

        // Act
        var response = await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None), Times.Never);
        response.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveCmcStraxPriceQuery()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                              TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);
        var latestSnapshot = new TokenSnapshot(2, 3, SnapshotType.Daily, latestSnapshotTime);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
            .ReturnsAsync(latestSnapshot);

        // Act
        try
        {
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveCmcStraxPriceQuery>(q => q.BlockTime == blockTime), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_ReturnsFalse_FailureGettingStraxPrice()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                              TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);
        var latestSnapshot = new TokenSnapshot(2, 3,SnapshotType.Daily, latestSnapshotTime);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
            .ReturnsAsync(latestSnapshot);

        // Act
        var response = await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None), Times.AtMostOnce);
        response.Should().BeFalse();
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveTokenSnapshotWithFilterQuery_ForEachSnapshotType()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                              TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);
        var latestSnapshot = new TokenSnapshot(2, 3, SnapshotType.Minute, latestSnapshotTime);
        const decimal price = 1.1m;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
            .ReturnsAsync(latestSnapshot);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None))
            .ReturnsAsync(price);

        // Act
        await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == token.Id &&
                                                                                                q.MarketId == 0 &&
                                                                                                q.DateTime == blockTime &&
                                                                                                q.SnapshotType == SnapshotType.Minute), CancellationToken.None), Times.Exactly(2));

        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == token.Id &&
                                                                                                q.MarketId == 0 &&
                                                                                                q.DateTime == blockTime &&
                                                                                                q.SnapshotType == SnapshotType.Hourly), CancellationToken.None), Times.Once);

        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == token.Id &&
                                                                                                q.MarketId == 0 &&
                                                                                                q.DateTime == blockTime &&
                                                                                                q.SnapshotType == SnapshotType.Daily), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCrsTokenSnapshotsCommand_Sends_MakeTokenSnapshotCommand_ForEachSnapshotType()
    {
        // Arrange
        DateTime blockTime = DateTime.UtcNow;
        DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
        const ulong blockHeight = 10;
        var token = new Token(1, Address.Cirrus, false, TokenConstants.Cirrus.Name, TokenConstants.Cirrus.Symbol, TokenConstants.Cirrus.Decimals,
                                 TokenConstants.Cirrus.Sats, TokenConstants.Cirrus.TotalSupply, blockHeight, blockHeight);
        var latestSnapshot = new TokenSnapshot(2, 3, SnapshotType.Daily, latestSnapshotTime);
        const decimal price = 1.1m;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), CancellationToken.None))
            .ReturnsAsync(token);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
            .ReturnsAsync(latestSnapshot);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None))
            .ReturnsAsync(price);

        // Act
        await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenSnapshotCommand>(q => q.BlockHeight == blockHeight), CancellationToken.None), Times.Exactly(3));
    }
}
