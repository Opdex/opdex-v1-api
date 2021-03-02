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
    public class RetrieveBurnEventsByTransactionIdQueryHandler : IRequestHandler<RetrieveBurnEventsByTransactionIdQuery, IEnumerable<BurnEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveBurnEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<BurnEvent>> Handle(RetrieveBurnEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var burnEvents = await _mediator.Send(new SelectBurnEventsByTransactionIdQuery(request.TransactionId), cancellationToken);

            return burnEvents;
        }
    }
}