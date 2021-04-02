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
    public class RetrieveTransferEventsByIdsQueryHandler : IRequestHandler<RetrieveTransferEventsByIdsQuery, IEnumerable<TransferEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransferEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<TransferEvent>> Handle(RetrieveTransferEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var transferEvents = await _mediator.Send(new SelectTransferEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return transferEvents;
        }
    }
}