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
    public class RetrievePoolCreatedEventsByIdsQueryHandler : IRequestHandler<RetrievePoolCreatedEventsByIdsQuery, IEnumerable<PoolCreatedEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrievePoolCreatedEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<PoolCreatedEvent>> Handle(RetrievePoolCreatedEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var poolCreatedEvents = await _mediator.Send(new SelectPoolCreatedEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return poolCreatedEvents;
        }
    }
}