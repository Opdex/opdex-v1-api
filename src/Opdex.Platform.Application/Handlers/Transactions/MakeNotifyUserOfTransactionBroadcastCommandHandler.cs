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

        private const int MaxRetries = 3; // Retry mempool times
        private const int Backoff = 5; // seconds prior to retry

        public MakeNotifyUserOfTransactionBroadcastCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(MakeNotifyUserOfTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            int attempt = 0;
            bool existsInMempool = false;

            // Retry up to MaxRetries times with the configured Backoff in seconds in between attempts
            while (++attempt <= MaxRetries)
            {
                try
                {
                    if (attempt > 1) await Task.Delay(TimeSpan.FromSeconds(Backoff));

                    existsInMempool = await _mediator.Send(new CallCirrusGetExistsInMempoolQuery(request.TransactionHash), cancellationToken);

                    if (!existsInMempool) continue;

                    break;
                }
                catch
                {
                    existsInMempool = false;
                }
            }

            // Do not notify if we never found the transaction in the mempool
            if (!existsInMempool) return false;

            await _mediator.Send(new NotifyUserOfBroadcastTransactionCommand(request.User, request.TransactionHash), cancellationToken);

            return true;
        }
    }
}
