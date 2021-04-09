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
    public class RetrieveSwapLogsByIdsQueryHandler : IRequestHandler<RetrieveSwapLogsByIdsQuery, IEnumerable<SwapLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveSwapLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<SwapLog>> Handle(RetrieveSwapLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var swapLogs = await _mediator.Send(new SelectSwapLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return swapLogs;
        }
    }
}