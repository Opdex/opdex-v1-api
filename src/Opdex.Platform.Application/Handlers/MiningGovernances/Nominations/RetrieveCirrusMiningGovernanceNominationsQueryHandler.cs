using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances.Nominations;

public class RetrieveCirrusMiningGovernanceNominationsQueryHandler
    : IRequestHandler<RetrieveCirrusMiningGovernanceNominationsQuery, IEnumerable<MiningGovernanceContractNominationSummary>>
{
    private readonly IMediator _mediator;

    public RetrieveCirrusMiningGovernanceNominationsQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<MiningGovernanceContractNominationSummary>> Handle(RetrieveCirrusMiningGovernanceNominationsQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusGetMiningGovernanceNominationsSummaryQuery(request.Address, request.BlockHeight), cancellationToken);
    }
}