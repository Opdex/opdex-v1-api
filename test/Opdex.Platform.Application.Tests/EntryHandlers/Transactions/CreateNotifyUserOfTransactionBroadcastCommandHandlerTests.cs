using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions;

public class CreateNotifyUserOfTransactionBroadcastCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;

    private readonly CreateNotifyUserOfTransactionBroadcastCommandHandler _handler;

    public CreateNotifyUserOfTransactionBroadcastCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new CreateNotifyUserOfTransactionBroadcastCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveTransactionByHashQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var request = new CreateNotifyUserOfTransactionBroadcastCommand(new Sha256(34283925829));

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTransactionByHashQuery>(
            q => q.Hash == request.TransactionHash && !q.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_TransactionAlreadyIndexed_DoNotNotify()
    {
        // Arrange
        var request = new CreateNotifyUserOfTransactionBroadcastCommand(new Sha256(34283925829));
        var transaction = new Transaction(1, new Sha256(5340958239), 2, 3, "PFrSHgtz2khDuciJdLAZtR2uKwgyXryMjM", "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", true, null, "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM");
        _mediatorMock.Setup(callTo =>
                callTo.Send(It.IsAny<RetrieveTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(transaction);

        // Act
        var notified = await _handler.Handle(request, CancellationToken.None);

        // Assert
        notified.Should().Be(false);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<MakeNotifyUserOfTransactionBroadcastCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RetrieveCirrusUnverifiedTransactionSenderByHashQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        _mediatorMock.Setup(callTo =>
                callTo.Send(It.IsAny<RetrieveTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Transaction)null);

        var request = new CreateNotifyUserOfTransactionBroadcastCommand(new Sha256(34283925829));

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveCirrusUnverifiedTransactionSenderByHashQuery>(
            q => q.TransactionHash == request.TransactionHash), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_SenderNotFound_DoNotNotify()
    {
        // Arrange
        _mediatorMock.Setup(callTo =>
                callTo.Send(It.IsAny<RetrieveTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Transaction)null);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusUnverifiedTransactionSenderByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Address.Empty);

        var request = new CreateNotifyUserOfTransactionBroadcastCommand(new Sha256(34283925829));

        // Act
        var notified = await _handler.Handle(request, CancellationToken.None);

        // Assert
        notified.Should().Be(false);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<MakeNotifyUserOfTransactionBroadcastCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SenderFound_Notify()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var sender = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh");

        _mediatorMock.Setup(callTo =>
                callTo.Send(It.IsAny<RetrieveTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Transaction)null);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusUnverifiedTransactionSenderByHashQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(sender);

        var request = new CreateNotifyUserOfTransactionBroadcastCommand(new Sha256(34283925829));

        // Act
        var notified = await _handler.Handle(request, cancellationToken);

        // Assert
        notified.Should().Be(true);
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeNotifyUserOfTransactionBroadcastCommand>(
            c => c.User == sender && c.TransactionHash == request.TransactionHash), cancellationToken), Times.Once);
    }
}
