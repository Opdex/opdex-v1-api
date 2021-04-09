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
    public class RetrieveTransferLogsByIdsQueryHandler : IRequestHandler<RetrieveTransferLogsByIdsQuery, IEnumerable<TransferLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransferLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<TransferLog>> Handle(RetrieveTransferLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var transferLogs = await _mediator.Send(new SelectTransferLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return transferLogs;
        }
    }
}