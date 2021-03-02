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
    public class RetrieveMintEventsByTransactionIdQueryHandler : IRequestHandler<RetrieveMintEventsByTransactionIdQuery, IEnumerable<MintEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveMintEventsByTransactionIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<MintEvent>> Handle(RetrieveMintEventsByTransactionIdQuery request, CancellationToken cancellationToken)
        {
            var mintEvents = await _mediator.Send(new SelectMintEventsByTransactionIdQuery(request.TransactionId), cancellationToken);

            return mintEvents;
        }
    }
}