using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Core.Application.Handlers.Transactions.TransactionLogs
{
    public class RetrieveApprovalLogsByIdsQueryHandler : IRequestHandler<RetrieveApprovalLogsByIdsQuery, IEnumerable<ApprovalLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveApprovalLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<ApprovalLog>> Handle(RetrieveApprovalLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var approvalLogs = await _mediator.Send(new SelectApprovalLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return approvalLogs;
        }
    }
}