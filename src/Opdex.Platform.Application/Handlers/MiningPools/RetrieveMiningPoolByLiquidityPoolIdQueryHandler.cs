using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningPools;

public class RetrieveMiningPoolByLiquidityPoolIdQueryHandler : IRequestHandler<RetrieveMiningPoolByLiquidityPoolIdQuery, MiningPool>
{
    private readonly IMediator _mediator;

    public RetrieveMiningPoolByLiquidityPoolIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<MiningPool> Handle(RetrieveMiningPoolByLiquidityPoolIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMiningPoolByLiquidityPoolIdQuery(request.LiquidityPoolId, request.FindOrThrow), cancellationToken);
    }
}