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
    public class RetrieveApprovalEventsByIdsQueryHandler : IRequestHandler<RetrieveApprovalEventsByIdsQuery, IEnumerable<ApprovalEvent>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveApprovalEventsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<ApprovalEvent>> Handle(RetrieveApprovalEventsByIdsQuery request, CancellationToken cancellationToken)
        {
            var approvalEvents = await _mediator.Send(new SelectApprovalEventsByIdsQuery(request.TransactionEvents), cancellationToken);

            return approvalEvents;
        }
    }
}