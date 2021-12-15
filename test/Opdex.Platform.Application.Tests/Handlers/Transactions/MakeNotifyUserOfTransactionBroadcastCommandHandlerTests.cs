using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions;

public class MakeNotifyUserOfTransactionBroadcastCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeNotifyUserOfTransactionBroadcastCommandHandlerWrapper _handler;

    public MakeNotifyUserOfTransactionBroadcastCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeNotifyUserOfTransactionBroadcastCommandHandlerWrapper(_mediator.Object);
    }

    [Fact]
    public async Task Handle_TransactionWasFoundInMempool_AttemptToNotifyUser()
    {
        // Arrange
        var request = new MakeNotifyUserOfTransactionBroadcastCommand("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", new Sha256(43594389025));
        var cancellationToken = new CancellationTokenSource().Token;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<NotifyUserOfBroadcastTransactionCommand>(
                                                   command => command.User == request.User && command.TxHash == request.TransactionHash), cancellationToken), Times.Once);
    }

    private class MakeNotifyUserOfTransactionBroadcastCommandHandlerWrapper : MakeNotifyUserOfTransactionBroadcastCommandHandler
    {
        public MakeNotifyUserOfTransactionBroadcastCommandHandlerWrapper(IMediator mediator) : base(mediator)
        {
        }

        public new Task Handle(MakeNotifyUserOfTransactionBroadcastCommand command, CancellationToken cancellationToken) =>
            base.Handle(command, cancellationToken);
    }
}
