using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Balances;

public class CreateAddressBalanceCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateAddressBalanceCommandHandler _handler;

    public CreateAddressBalanceCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateAddressBalanceCommandHandler(_mediator.Object);
    }

    [Fact]
    public void CreateAddressBalanceCommand_InvalidWalletAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateAddressBalanceCommand(null, token, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Wallet address must be provided.");
    }

    [Fact]
    public void CreateAddressBalanceCommand_InvalidToken_ThrowsArgumentNullException()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateAddressBalanceCommand(walletAddress, null, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
    }

    [Fact]
    public void CreateAddressBalanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";

        // Act
        void Act() => new CreateAddressBalanceCommand(walletAddress, token, 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CreateAddressBalanceCommand_Sends_RetrieveTokenByAddressQuery()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateAddressBalanceCommand(walletAddress, token, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == token),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAddressBalanceCommand_Sends_RetrieveAddressBalanceByOwnerAndTokenQuery()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;

        var tokenResponse = new Token(1, token, "TokenName", "Symbol", 8, 100_000_00, 10000000000, 4, 5);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // Act
        try
        {
            await _handler.Handle(new CreateAddressBalanceCommand(walletAddress, token, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalanceByOwnerAndTokenQuery>(q => q.Owner == walletAddress &&
                                                                                                      q.TokenId == tokenResponse.Id &&
                                                                                                      q.FindOrThrow == false),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAddressBalanceCommand_DoesNotSend_MakeAddressBalanceCommand_BalanceIsNewerThanRequestedBlockHeight()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;
        const ulong balanceModifiedBlock = 11;

        var tokenResponse = new Token(2, token, "TokenName", "Symbol", 8, 100_000_00, 10000000000, 4, 5);

        var balance = new AddressBalance(1, tokenResponse.Id, walletAddress, 10, 3, balanceModifiedBlock);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        // Act
        var response = await _handler.Handle(new CreateAddressBalanceCommand(walletAddress, token, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeAddressBalanceCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        response.Should().Be(balance.Id);
    }

    [Fact]
    public async Task CreateAddressBalanceCommand_Sends_MakeAddressBalanceCommand_ExistingAddressBalance()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;

        var tokenResponse = new Token(2, token, "TokenName", "Symbol", 8, 100_000_00, 10000000000, 4, 5);

        var balance = new AddressBalance(1, tokenResponse.Id, walletAddress, 10, 3, blockHeight);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        // Act
        await _handler.Handle(new CreateAddressBalanceCommand(walletAddress, token, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeAddressBalanceCommand>(q => q.Token == token &&
                                                                                     q.BlockHeight == blockHeight &&
                                                                                     q.AddressBalance.Id > 0),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAddressBalanceCommand_Sends_MakeAddressBalanceCommand_NewAddressBalance()
    {
        // Arrange
        Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const ulong blockHeight = 10;

        var tokenResponse = new Token(2, token, "TokenName", "Symbol", 8, 100_000_00, 10000000000, 4, 5);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(null);

        // Act
        await _handler.Handle(new CreateAddressBalanceCommand(walletAddress, token, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeAddressBalanceCommand>(q => q.Token == token &&
                                                                                     q.BlockHeight == blockHeight &&
                                                                                     q.AddressBalance.Id == 0 &&
                                                                                     q.AddressBalance.TokenId == tokenResponse.Id &&
                                                                                     q.AddressBalance.Owner == walletAddress &&
                                                                                     q.AddressBalance.Balance == UInt256.Zero),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}
