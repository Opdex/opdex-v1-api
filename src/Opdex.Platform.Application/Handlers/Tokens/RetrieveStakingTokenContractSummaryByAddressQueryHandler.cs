using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveStakingTokenContractSummaryByAddressQueryHandler
        : IRequestHandler<RetrieveStakingTokenContractSummaryByAddressQuery, StakingTokenContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveStakingTokenContractSummaryByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<StakingTokenContractSummary> Handle(RetrieveStakingTokenContractSummaryByAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetStakingTokenSummaryByAddressQuery(request.Address), cancellationToken);
        }
    }
}
