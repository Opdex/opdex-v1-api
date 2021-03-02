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
    public class RetrieveTransferEventsByTransactionIdQueryHandler : IRequestHandler<RetrieveTransferEventsByTransactionIdQuery, IEnumerable<TransferEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransferEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<TransferEvent>> Handle(RetrieveTransferEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var transferEvents = await _mediator.Send(new SelectTransferEventsByTransactionIdQuery(request.TransactionId), cancellationToken);

            return transferEvents;
        }
    }
}