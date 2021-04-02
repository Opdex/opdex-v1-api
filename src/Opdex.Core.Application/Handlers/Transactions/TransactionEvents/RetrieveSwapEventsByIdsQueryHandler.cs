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
    public class RetrieveSwapEventsByIdsQueryHandler : IRequestHandler<RetrieveSwapEventsByIdsQuery, IEnumerable<SwapEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveSwapEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<SwapEvent>> Handle(RetrieveSwapEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var swapEvents = await _mediator.Send(new SelectSwapEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return swapEvents;
        }
    }
}