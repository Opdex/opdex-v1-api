using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Platform.Application.Handlers.Transactions.TransactionLogs;

public class RetrieveTransactionLogsByTransactionIdQueryHandler
    : IRequestHandler<RetrieveTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>
{
    private readonly IMediator _mediator;
        
    public RetrieveTransactionLogsByTransactionIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
        
    public async Task<IEnumerable<TransactionLog>> Handle(RetrieveTransactionLogsByTransactionIdQuery request, CancellationToken cancellationToken)
    {
        var transactionLogs = await _mediator.Send(new SelectTransactionLogsByTransactionIdQuery(request.TransactionId), cancellationToken);

        return transactionLogs;
    }
}