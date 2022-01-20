using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;

public class ProcessLiquidityPoolSnapshotRefreshCommandHandler : IRequestHandler<ProcessLiquidityPoolSnapshotRefreshCommand, Unit>
{
    private readonly IMediator _mediator;

    public ProcessLiquidityPoolSnapshotRefreshCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Unit> Handle(ProcessLiquidityPoolSnapshotRefreshCommand request, CancellationToken cancellationToken)
    {
        // Retrieve liquidity pool snapshot
        var lpSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(request.LiquidityPoolId,
                                                                                               request.BlockTime,
                                                                                               request.SnapshotType));

        // Process new token snapshot
        var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(request.MarketId, request.SrcToken, request.SnapshotType,
                                                                             request.BlockTime, request.CrsUsd, lpSnapshot.Reserves.Crs.Close,
                                                                             lpSnapshot.Reserves.Src.Close, request.BlockHeight));

        var stakingUsd = request.StakingTokenUsd ?? 0m;

        // Reset stale snapshots
        if (lpSnapshot.EndDate < request.BlockTime)
        {
            // Process latest lp snapshot
            lpSnapshot.ResetStaleSnapshot(request.CrsUsd, stakingUsd, request.SrcToken.Sats, request.BlockTime);
        }
        else // refresh existing snapshot USD amounts
        {
            lpSnapshot.RefreshSnapshotFiatAmounts(request.CrsUsd, stakingUsd);
        }

        await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(lpSnapshot, request.BlockHeight));

        // Process latest lp token snapshot
        var lptUsd = await _mediator.Send(new ProcessLpTokenSnapshotCommand(request.MarketId, request.LpToken, lpSnapshot.Reserves.Usd.Close,
                                                                            request.SnapshotType, request.BlockTime, request.BlockHeight));

        return Unit.Value;
    }
}
