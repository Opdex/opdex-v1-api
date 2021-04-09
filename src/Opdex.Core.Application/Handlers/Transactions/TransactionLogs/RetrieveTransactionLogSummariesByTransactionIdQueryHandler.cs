using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Core.Application.Handlers.Transactions.TransactionLogs
{
    public class RetrieveTransactionLogSummariesByTransactionIdQueryHandler 
        : IRequestHandler<RetrieveTransactionLogSummariesByTransactionIdQuery, List<TransactionLogSummary>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransactionLogSummariesByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<List<TransactionLogSummary>> Handle(RetrieveTransactionLogSummariesByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectTransactionLogSummariesByTransactionIdQuery(request.TransactionId);
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToList();
        }
    }
}