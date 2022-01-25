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
    private readonly IMediator _mediator;

    public RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Address> Handle(RetrieveCirrusUnverifiedTransactionSenderByHashQuery request, CancellationToken cancellationToken)
    {
        var rawTransaction = await _mediator.Send(new CallCirrusGetRawTransactionQuery(request.TransactionHash), cancellationToken);

        return rawTransaction.Vout.SelectMany(vOut => vOut?.ScriptPubKey.Addresses).FirstOrDefault();
    }
}
