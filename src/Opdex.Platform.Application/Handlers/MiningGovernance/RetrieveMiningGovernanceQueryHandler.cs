using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernance;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance;

namespace Opdex.Platform.Application.Handlers.MiningGovernance
{
    public class RetrieveMiningGovernanceQueryHandler : IRequestHandler<RetrieveMiningGovernanceQuery, Domain.MiningGovernance>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernanceQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<Domain.MiningGovernance> Handle(RetrieveMiningGovernanceQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningGovernanceQuery(request.FindOrThrow), cancellationToken);
        }
    }
}