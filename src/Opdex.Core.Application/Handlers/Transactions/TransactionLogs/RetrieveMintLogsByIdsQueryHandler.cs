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
    public class RetrieveMintLogsByIdsQueryHandler : IRequestHandler<RetrieveMintLogsByIdsQuery, IEnumerable<MintLog>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveMintLogsByIdsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<IEnumerable<MintLog>> Handle(RetrieveMintLogsByIdsQuery request, CancellationToken cancellationToken)
        {
            var mintLogs = await _mediator.Send(new SelectMintLogsByIdsQuery(request.TransactionLogs), cancellationToken);

            return mintLogs;
        }
    }
}