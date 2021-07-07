using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class RetrieveMiningGovernanceQueryHandler : IRequestHandler<RetrieveMiningGovernanceQuery, MiningGovernance>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningGovernanceQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<MiningGovernance> Handle(RetrieveMiningGovernanceQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningGovernanceQuery(request.FindOrThrow), cancellationToken);
        }
    }
}
