using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Transactions;

public class CallCirrusGetRawTransactionQueryHandler : IRequestHandler<CallCirrusGetRawTransactionQuery, RawTransactionDto>
{
    private const int MaxRetries = 5;
    private const int Backoff = 3;
    private readonly INodeModule _nodeModule;

    public CallCirrusGetRawTransactionQueryHandler(INodeModule nodeModule)
    {
        _nodeModule = nodeModule;
    }

    public async Task<RawTransactionDto> Handle(CallCirrusGetRawTransactionQuery request, CancellationToken cancellationToken)
    {
        int attempt = 0;
        RawTransactionDto rawTransaction = null;

        // Retry up to MaxRetries times with the configured Backoff in seconds in between attempts
        while (rawTransaction == null && ++attempt <= MaxRetries && !cancellationToken.IsCancellationRequested)
        {
            if (attempt > 1) await Task.Delay(TimeSpan.FromSeconds(Backoff), cancellationToken);

            try
            {
                rawTransaction = await _nodeModule.GetRawTransactionAsync(request.TransactionHash, cancellationToken);
            }
            catch (Exception)
            {
                // Nothing to do here, logs may not really be useful
            }
        }

        if (rawTransaction == null && request.FindOrThrow)
        {
            throw new NotFoundException("Raw transaction not found.");
        }

        return rawTransaction;
    }
}
