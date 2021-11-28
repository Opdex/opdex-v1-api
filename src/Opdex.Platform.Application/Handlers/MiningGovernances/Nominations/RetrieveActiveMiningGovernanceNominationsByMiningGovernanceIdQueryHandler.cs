using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances.Nominations
{
    public class RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler
        : IRequestHandler<RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>
    {
        private readonly IMediator _mediator;

        public RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<MiningGovernanceNomination>> Handle(RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(request.MiningGovernanceId), cancellationToken);
        }
    }
}
