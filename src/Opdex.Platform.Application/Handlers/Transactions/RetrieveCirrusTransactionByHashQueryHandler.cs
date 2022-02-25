using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models.Transactions;
using Polly;
using Polly.Retry;
using System.Net.Http;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class RetrieveCirrusTransactionByHashQueryHandler : IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>
{
    private readonly IMediator _mediator;

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(x));

    public RetrieveCirrusTransactionByHashQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Transaction> Handle(RetrieveCirrusTransactionByHashQuery request, CancellationToken cancellationToken)
    {
        var query = await _retryPolicy.ExecuteAndCaptureAsync(
            async () => await _mediator.Send(new CallCirrusGetTransactionByHashQuery(request.TxHash), cancellationToken));
        return query.Result;
    }
}
