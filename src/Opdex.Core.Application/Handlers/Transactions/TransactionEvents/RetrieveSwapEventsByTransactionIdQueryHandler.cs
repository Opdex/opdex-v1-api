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
    public class RetrieveSwapEventsByTransactionIdQueryHandler : IRequestHandler<RetrieveSwapEventsByTransactionIdQuery, IEnumerable<SwapEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveSwapEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<SwapEvent>> Handle(RetrieveSwapEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var swapEvents = await _mediator.Send(new SelectSwapEventsByTransactionIdQuery(request.TransactionId), cancellationToken);

            return swapEvents;
        }
    }
}