using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class RetrieveCirrusExistsInMempoolQueryHandler : IRequestHandler<RetrieveCirrusExistsInMempoolQuery, bool>
{
    private readonly IMediator _mediator;
    private const int MaxRetries = 3; // Retry mempool times
    private const int Backoff = 5; // seconds prior to retry

    public RetrieveCirrusExistsInMempoolQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(RetrieveCirrusExistsInMempoolQuery request, CancellationToken cancellationToken)
    {
        int attempt = 0;

        // Retry up to MaxRetries times with the configured Backoff in seconds in between attempts
        while (++attempt <= MaxRetries && !cancellationToken.IsCancellationRequested)
        {
            if (attempt > 1) await Task.Delay(TimeSpan.FromSeconds(Backoff), cancellationToken);

            bool existsInMempool = await _mediator.Send(new CallCirrusGetExistsInMempoolQuery(request.TransactionHash), cancellationToken);

            if (existsInMempool) return true;
        }

        return false;
    }
}
