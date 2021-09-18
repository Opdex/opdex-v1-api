using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class MakeNotifyUserOfTransactionBroadcastCommandHandler : IRequestHandler<MakeNotifyUserOfTransactionBroadcastCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakeNotifyUserOfTransactionBroadcastCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(MakeNotifyUserOfTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            var existsInMempool = await _mediator.Send(new CallCirrusGetExistsInMempoolQuery(request.TransactionHash), cancellationToken);
            if (!existsInMempool) return false;

            await _mediator.Send(new NotifyUserOfBroadcastTransactionCommand(request.User, request.TransactionHash), cancellationToken);
            return true;
        }
    }
}
