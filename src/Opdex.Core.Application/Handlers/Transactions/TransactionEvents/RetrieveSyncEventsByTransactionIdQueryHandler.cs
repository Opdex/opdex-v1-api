using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;

namespace Opdex.Core.Application.Handlers.Transactions.TransactionEvents
{
    public class RetrieveSyncEventsByTransactionIdQueryHandler : IRequestHandler<RetrieveSyncEventsByTransactionIdQuery, IEnumerable<SyncEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveSyncEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<SyncEvent>> Handle(RetrieveSyncEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var syncEvents = await _mediator.Send(new SelectSyncEventsByTransactionIdQuery(request.TransactionEvents), cancellationToken);

            return syncEvents;
        }
    }
}