using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var logTypes = request.EventTypes.SelectMany(ev => ev.GetLogTypes()).Distinct().Cast<uint>();

            return _mediator.Send(new SelectTransactionsWithFilterQuery(request.Wallet, logTypes, request.Contracts,
                                                                        request.Direction, request.Limit,
                                                                        request.Next, request.Previous), cancellationToken);
        }
    }
}
