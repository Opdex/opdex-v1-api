using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class RetrieveTransactionsWithFilterQueryHandler : IRequestHandler<RetrieveTransactionsWithFilterQuery, List<Transaction>>
    {
        private readonly IMediator _mediator;

        public RetrieveTransactionsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<List<Transaction>> Handle(RetrieveTransactionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectTransactionsWithFilterQuery(request.Wallet, request.IncludeEvents, request.ExcludeEvents,
                                                                        request.Contracts, request.Direction, request.Limit,
                                                                        request.Next, request.Previous), cancellationToken);
        }
    }
}
