using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using System;
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

            if (transaction is null || transaction.Vout?.Length == 0 || transaction.Vout?[0].ScriptPubKey?.Addresses?.Length != 1) continue;

            return transaction.Vout[0].ScriptPubKey.Addresses[0];
        }

        return Address.Empty;

    }
}
