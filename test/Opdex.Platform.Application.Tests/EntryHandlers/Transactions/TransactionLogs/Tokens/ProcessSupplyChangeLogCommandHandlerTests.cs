using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions.TransactionLogs.Tokens;

public class ProcessSupplyChangeLogCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProcessSupplyChangeLogCommandHandler _handler;

    public ProcessSupplyChangeLogCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new ProcessSupplyChangeLogCommandHandler(_mediatorMock.Object, new NullLogger<ProcessSupplyChangeLogCommandHandler>());
    }

    [Fact]
    public async Task Handle_RetrieveTokenByAddressQuery_Send()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = "100000000000";
        var log = new SupplyChangeLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessSupplyChangeLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveTokenByAddressQuery>(q => q.Address == log.Contract && !q.FindOrThrow), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_TokenNotFound_ReturnFalse()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = "100000000000";
        var log = new SupplyChangeLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessSupplyChangeLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Token)null);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().Be(false);
    }

    [Fact]
    public async Task Handle_ModifiedAtLaterBlock_ReturnTrue()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = "100000000000";
        var log = new SupplyChangeLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessSupplyChangeLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, command.BlockHeight + 50);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().Be(true);
    }

    [Fact]
    public async Task Handle_ModifiedAtEarlierBlock_UpdateAndPersist()
    {
        // Arrange
        UInt256 updatedSupply = 100000000000;
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = updatedSupply.ToString();
        var log = new SupplyChangeLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessSupplyChangeLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, command.BlockHeight - 70);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<MakeTokenCommand>(c => c.Token.Id == token.Id
                && c.Token.TotalSupply == updatedSupply && c.Token.ModifiedBlock == command.BlockHeight), CancellationToken.None), Times.Once);
    }
}
