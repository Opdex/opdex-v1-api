using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningGovernances;

public class RetrieveMiningGovernanceByTokenIdQueryHandler : IRequestHandler<RetrieveMiningGovernanceByTokenIdQuery, MiningGovernance>
{
    private readonly IMediator _mediator;

    public RetrieveMiningGovernanceByTokenIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<MiningGovernance> Handle(RetrieveMiningGovernanceByTokenIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMiningGovernanceByTokenIdQuery(request.TokenId, request.FindOrThrow), cancellationToken);
    }
}