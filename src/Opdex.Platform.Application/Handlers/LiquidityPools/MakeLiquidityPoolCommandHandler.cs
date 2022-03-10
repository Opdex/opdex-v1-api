using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools;

public class MakeLiquidityPoolCommandHandler : IRequestHandler<MakeLiquidityPoolCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeLiquidityPoolCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeLiquidityPoolCommand request, CancellationToken cancellationToken)
    {
        var poolId = await _mediator.Send(new PersistLiquidityPoolCommand(request.LiquidityPool), CancellationToken.None);
        await _mediator.Send(new ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(request.LiquidityPool.MarketId, request.LiquidityPool.CreatedBlock), CancellationToken.None);
        return poolId;
    }
}
