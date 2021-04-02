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
    public class RetrieveSyncEventsByIdsQueryHandler : IRequestHandler<RetrieveSyncEventsByIdsQuery, IEnumerable<SyncEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveSyncEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<SyncEvent>> Handle(RetrieveSyncEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var syncEvents = await _mediator.Send(new SelectSyncEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return syncEvents;
        }
    }
}