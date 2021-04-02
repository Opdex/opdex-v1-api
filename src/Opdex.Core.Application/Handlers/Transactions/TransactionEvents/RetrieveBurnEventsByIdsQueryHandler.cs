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
    public class RetrieveBurnEventsByIdsQueryHandler : IRequestHandler<RetrieveBurnEventsByIdsQuery, IEnumerable<BurnEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveBurnEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<BurnEvent>> Handle(RetrieveBurnEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var burnEvents = await _mediator.Send(new SelectBurnEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return burnEvents;
        }
    }
}