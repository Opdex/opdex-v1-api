using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Governances
{
    public class RetrieveCirrusMiningGovernanceNominationsQueryHandler
        : IRequestHandler<RetrieveCirrusMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        private readonly IMediator _mediator;

        public RetrieveCirrusMiningGovernanceNominationsQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<MiningGovernanceNominationCirrusDto>> Handle(RetrieveCirrusMiningGovernanceNominationsQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetMiningGovernanceSummaryNominationsQuery(request.Address), cancellationToken);
        }
    }
}
