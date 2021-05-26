using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernance;

namespace Opdex.Platform.Application.Handlers.MiningGovernance
{
    public class RetrieveMiningGovernanceContractSummaryByAddressQueryHandler
        : IRequestHandler<RetrieveMiningGovernanceContractSummaryByAddressQuery, MiningGovernanceContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernanceContractSummaryByAddressQueryHandler(IMediator mediator)
        {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
            
        public Task<MiningGovernanceContractSummary> Handle(RetrieveMiningGovernanceContractSummaryByAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetMiningGovernanceSummaryByAddressQuery(request.Address), cancellationToken);
        }
    }
}