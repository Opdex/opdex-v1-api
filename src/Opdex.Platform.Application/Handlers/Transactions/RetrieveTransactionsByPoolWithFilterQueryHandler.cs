using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class RetrieveTransactionsByPoolWithFilterQueryHandler 
        : IRequestHandler<RetrieveTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTransactionsByPoolWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<Transaction>> Handle(RetrieveTransactionsByPoolWithFilterQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectTransactionsByPoolWithFilterQuery(request.PoolAddress, request.EventTypes);

            return await _mediator.Send(query, cancellationToken);
        }
    }
}