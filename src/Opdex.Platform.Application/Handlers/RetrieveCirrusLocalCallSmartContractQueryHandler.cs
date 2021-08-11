using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Platform.Application.Handlers
{
    public class RetrieveCirrusLocalCallSmartContractQueryHandler : IRequestHandler<RetrieveCirrusLocalCallSmartContractQuery, TransactionQuote>
    {
        private readonly IMediator _mediator;

        public RetrieveCirrusLocalCallSmartContractQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<TransactionQuote> Handle(RetrieveCirrusLocalCallSmartContractQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusLocalCallSmartContractMethodQuery(request.QuoteRequest), cancellationToken);
        }
    }
}
