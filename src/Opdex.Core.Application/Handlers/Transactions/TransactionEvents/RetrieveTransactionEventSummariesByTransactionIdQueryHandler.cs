using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;

namespace Opdex.Core.Application.Handlers.Transactions.TransactionEvents
{
    public class RetrieveTransactionEventSummariesByTransactionIdQueryHandler 
        : IRequestHandler<RetrieveTransactionEventSummariesByTransactionIdQuery, List<TransactionEventSummary>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransactionEventSummariesByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<List<TransactionEventSummary>> Handle(RetrieveTransactionEventSummariesByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectTransactionEventSummariesByTransactionIdQuery(request.TransactionId);
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToList();
        }
    }
}