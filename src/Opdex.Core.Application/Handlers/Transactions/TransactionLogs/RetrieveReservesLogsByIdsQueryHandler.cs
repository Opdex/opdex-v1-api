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
    public class RetrieveReservesLogsByIdsQueryHandler : IRequestHandler<RetrieveReservesLogsByIdsQuery, IEnumerable<ReservesLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveReservesLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<ReservesLog>> Handle(RetrieveReservesLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var syncLogs = await _mediator.Send(new SelectReservesLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return syncLogs;
        }
    }
}