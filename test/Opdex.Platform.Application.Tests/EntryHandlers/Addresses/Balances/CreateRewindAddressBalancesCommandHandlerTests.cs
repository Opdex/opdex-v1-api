using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Balances;

public class CreateRewindAddressBalancesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindAddressBalancesCommandHandler _handler;

    public CreateRewindAddressBalancesCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindAddressBalancesCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindAddressBalancesCommandHandler>>());
    }

    [Fact]
    public void CreateRewindAddressBalances_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindAddressBalancesCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindAddressBalances_Sends_RetrieveAddressBalancesByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindAddressBalancesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindAddressBalances_Sends_RetrieveTokenByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        var balance = new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AddressBalance> { balance });

        // Act
        try
        {
            await _handler.Handle(new CreateRewindAddressBalancesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(q => q.TokenId == balance.TokenId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindAddressBalances_Sends_MakeAddressBalanceCommand()
    {
        // Arrange
        const ulong rewindHeight = 10;
        var balances = new List<AddressBalance>
        {
            new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4),
            new AddressBalance(2, 2, "P5uJYUcmAsqAEgUXjBJPuCXfcNKdN28FQf", 5, 3, 4)
        };

        var tokenResponse = new Token(2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", false, "TokenName", "Symbol", 8, 100_000_00, 10000000000, new TokenSummary(5, 10, 50), 4, 5);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balances);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindAddressBalancesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var balance in balances)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeAddressBalanceCommand>(q => q.AddressBalance.Equals(balance) &&
                                                                                         q.Token == tokenResponse.Address &&
                                                                                         q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
