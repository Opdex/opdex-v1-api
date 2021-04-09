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
    public class RetrieveBurnLogsByIdsQueryHandler : IRequestHandler<RetrieveBurnLogsByIdsQuery, IEnumerable<BurnLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveBurnLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<BurnLog>> Handle(RetrieveBurnLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var burnLogs = await _mediator.Send(new SelectBurnLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return burnLogs;
        }
    }
}