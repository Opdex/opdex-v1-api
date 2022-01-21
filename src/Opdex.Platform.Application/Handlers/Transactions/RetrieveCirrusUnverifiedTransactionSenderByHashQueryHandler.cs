using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler : IRequestHandler<RetrieveCirrusUnverifiedTransactionSenderByHashQuery, Address>
{
    private const int MaxRetries = 3;
    private const int Backoff = 5;

    private readonly IMediator _mediator;

    public RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Address> Handle(RetrieveCirrusUnverifiedTransactionSenderByHashQuery request, CancellationToken cancellationToken)
    {
        int attempt = 0;

        // Retry up to MaxRetries times with the configured Backoff in seconds in between attempts
        while (++attempt <= MaxRetries && !cancellationToken.IsCancellationRequested)
        {
            if (attempt > 1) await Task.Delay(TimeSpan.FromSeconds(Backoff), cancellationToken);
            var transaction = await _mediator.Send(new CallCirrusGetRawTransactionQuery(request.TransactionHash), cancellationToken);

            if (transaction is null) continue;

            return transaction.Vout.SelectMany(vOut => vOut?.ScriptPubKey.Addresses).FirstOrDefault();
        }

        return Address.Empty;

    }
}
