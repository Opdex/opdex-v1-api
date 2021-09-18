using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions
{
    public class MakeNotifyUserOfTransactionBroadcastCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeNotifyUserOfTransactionBroadcastCommandHandler _handler;

        public MakeNotifyUserOfTransactionBroadcastCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeNotifyUserOfTransactionBroadcastCommandHandler(_mediator.Object);
        }

        [Fact]
        public async Task Handle_CallCirrusGetExistsInMempoolQuery_Send()
        {
            // Arrange
            var transactionHash = "87ff7bf5dccd3683ef2925ff2ce9148766eb1c0cb97594d612d7b74e8214ba80";
            var request = new MakeNotifyUserOfTransactionBroadcastCommand("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", transactionHash);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetExistsInMempoolQuery>(query => query.TransactionHash == transactionHash), cancellationToken),
                             Times.Once);
        }

        [Fact]
        public async Task Handle_TransactionNotFoundInMempool_DoNotNotifyUser()
        {
            // Arrange
            var request = new MakeNotifyUserOfTransactionBroadcastCommand("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "87ff7bf5dccd3683ef2925ff2ce9148766eb1c0cb97594d612d7b74e8214ba80");

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().Be(false);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<NotifyUserOfBroadcastTransactionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_TransactionWasFoundInMempool_AttemptToNotifyUser()
        {
            // Arrange
            var request = new MakeNotifyUserOfTransactionBroadcastCommand("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "87ff7bf5dccd3683ef2925ff2ce9148766eb1c0cb97594d612d7b74e8214ba80");
            var cancellationToken = new CancellationTokenSource().Token;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.Should().Be(true);
            _mediator.Verify(callTo => callTo.Send(It.Is<NotifyUserOfBroadcastTransactionCommand>(
                command => command.User == request.User && command.TxHash == command.TxHash), cancellationToken), Times.Once);
        }
    }
}
