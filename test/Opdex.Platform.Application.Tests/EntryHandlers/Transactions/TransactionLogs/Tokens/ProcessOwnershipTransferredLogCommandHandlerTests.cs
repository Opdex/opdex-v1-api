using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions.TransactionLogs.Tokens;

public class ProcessOwnershipTransferredLogCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProcessOwnershipTransferredLogCommandHandler _handler;

    public ProcessOwnershipTransferredLogCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new ProcessOwnershipTransferredLogCommandHandler(_mediatorMock.Object, new NullLogger<ProcessOwnershipTransferredLogCommandHandler>());
    }

    [Fact]
    public async Task Handle_RetrieveTokenByAddressQuery_Send()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

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
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Token)null);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().Be(false);
    }

    [Fact]
    public async Task Handle_RetrieveTokenWrappedByTokenIdQuery_Send()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (Exception)
        {
            // ignore
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveTokenWrappedByTokenIdQuery>(q => q.TokenId == token.Id && q.FindOrThrow), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ErrorWrappedTokenNotFound_ReturnFalse()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenWrappedByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .Throws<NotFoundException>();

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
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = "POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var tokenWrapped = new TokenWrapped(1, token.Id, "Pb0mc7Nz9osaj4kc77HiBtV8L6DVGVV8kRb", ExternalChainType.Ethereum, "0x514910771af9ca656af840dff83e8264ecf986ca", 342, command.BlockHeight + 50);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenWrappedByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenWrapped);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().Be(true);
    }

    [Fact]
    public async Task Handle_ModifiedAtEarlierBlock_UpdateAndPersist()
    {
        // Arrange
        Address newOwner = new Address("POLp2uVqojah5kcXzHiBtV8LVDVGVAgvk4");
        dynamic txLog = new ExpandoObject();
        txLog.previousOwner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.newOwner = newOwner.ToString();
        var log = new OwnershipTransferredLog(txLog, "PV2p2k1Vqojah5kcX7HiBtV8L6DVGVAgvj5", 5);

        var command = new ProcessOwnershipTransferredLogCommand(log, new Address("PMsifMXrr2uNEL5AQD1LpiYTRFiRTA8uZU"), 5000000);

        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var tokenWrapped = new TokenWrapped(1, token.Id, "Pb0mc7Nz9osaj4kc77HiBtV8L6DVGVV8kRb", ExternalChainType.Ethereum, "0x514910771af9ca656af840dff83e8264ecf986ca", 342, command.BlockHeight - 70);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenWrappedByTokenIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenWrapped);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<MakeTokenWrappedCommand>(c => c.Wrapped.Id == tokenWrapped.Id
                && c.Wrapped.Owner == newOwner && c.Wrapped.ModifiedBlock == command.BlockHeight), CancellationToken.None), Times.Once);
    }
}
