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
    public class RetrieveLiquidityPoolCreatedLogsByIdsQueryHandler : IRequestHandler<RetrieveLiquidityPoolCreatedLogsByIdsQuery, IEnumerable<LiquidityPoolCreatedLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveLiquidityPoolCreatedLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<LiquidityPoolCreatedLog>> Handle(RetrieveLiquidityPoolCreatedLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var poolCreatedLogs = await _mediator.Send(new SelectLiquidityPoolCreatedLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return poolCreatedLogs;
        }
    }
}