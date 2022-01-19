using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers;

public class ProcessDailySnapshotRefreshCommandHandler : IRequestHandler<ProcessDailySnapshotRefreshCommand, Unit>
{
    private readonly IMediator _mediator;

    public ProcessDailySnapshotRefreshCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Unit> Handle(ProcessDailySnapshotRefreshCommand request, CancellationToken cancellationToken)
    {
        var blockTime = request.BlockTime.ToStartOf(SnapshotType.Daily);
        var snapshotTypes = new List<SnapshotType> {SnapshotType.Hourly, SnapshotType.Daily};

        var markets = await _mediator.Send(new RetrieveAllMarketsQuery());

        foreach (var market in markets)
        {
            var marketPools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(new LiquidityPoolsCursor(market.Address)));
            var stakingTokenUsd = 0m;

            // Process staking tokens and their liquidity pools first
            if (market.IsStakingMarket)
            {
                var stakingToken = await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId, findOrThrow: false));

                if (stakingToken == null) continue;

                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(stakingToken.Id, market.Id, findOrThrow: false));

                if (liquidityPool == null) continue;

                var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId, findOrThrow: false));

                if (lpToken == null) continue;

                foreach (var snapshotType in snapshotTypes)
                {
                    await _mediator.Send(new ProcessLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, market.Id, stakingToken, lpToken,
                                                                                             request.CrsUsd, snapshotType, blockTime,
                                                                                             request.BlockHeight));
                }

                var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingToken.Id, market.Id,
                                                                                                         blockTime, SnapshotType.Daily));
                stakingTokenUsd = stakingTokenSnapshot.Price.Close;
            }

            // Every pool excluding the staking token and it's liquidity pool
            foreach(var liquidityPool in marketPools.Where(mp => mp.SrcTokenId != market.StakingTokenId))
            {
                var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId, findOrThrow: false));
                var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId, findOrThrow: false));

                if (srcToken == null || lpToken == null) continue;

                foreach (var snapshotType in snapshotTypes)
                {
                    await _mediator.Send(new ProcessLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, market.Id, srcToken, lpToken,
                                                                                             request.CrsUsd, snapshotType, blockTime,
                                                                                             request.BlockHeight, stakingTokenUsd));
                }
            }

            // Process market snapshot
            await _mediator.Send(new ProcessMarketSnapshotsCommand(market, blockTime, request.BlockHeight));
        }

        return Unit.Value;
    }
}
