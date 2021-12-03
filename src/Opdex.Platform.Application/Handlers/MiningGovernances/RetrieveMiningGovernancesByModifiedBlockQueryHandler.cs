using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances;

public class RetrieveMiningGovernancesByModifiedBlockQueryHandler
    : IRequestHandler<RetrieveMiningGovernancesByModifiedBlockQuery, IEnumerable<MiningGovernance>>
{
    private readonly IMediator _mediator;

    public RetrieveMiningGovernancesByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<MiningGovernance>> Handle(RetrieveMiningGovernancesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMiningGovernancesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}