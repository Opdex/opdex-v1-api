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
    public class RetrievePairCreatedEventsByTransactionIdQueryHandler : IRequestHandler<RetrievePairCreatedEventsByTransactionIdQuery, IEnumerable<PairCreatedEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrievePairCreatedEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<PairCreatedEvent>> Handle(RetrievePairCreatedEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var pairCreatedEvents = await _mediator.Send(new SelectPairCreatedEventsByTransactionIdQuery(request.TransactionEvents), cancellationToken);

            return pairCreatedEvents;
        }
    }
}