using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers
{
    public class ProcessDailySnapshotRefreshCommandHandler : IRequestHandler<ProcessDailySnapshotRefreshCommand, Unit>
    {
        private readonly IMediator _mediator;

        public ProcessDailySnapshotRefreshCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessDailySnapshotRefreshCommand request, CancellationToken cancellationToken)
        {
            var blockTime = request.Blocktime.ToStartOf(SnapshotType.Daily);
            var snapshotTypes = new List<SnapshotType> {SnapshotType.Hourly, SnapshotType.Daily};

            var markets = await _mediator.Send(new RetrieveAllMarketsQuery());

            var tokenList = await _mediator.Send(new RetrieveAllTokensQuery());
            var tokens = tokenList.ToDictionary(k => k.Id);

            foreach (var market in markets)
            {
                var marketPools = await _mediator.Send(new RetrieveAllPoolsByMarketIdQuery(market.Id));
                var stakingTokenUsd = 0m;

                if (market.IsStakingMarket)
                {
                    if (!tokens.TryGetValue(market.StakingTokenId.GetValueOrDefault(), out var stakingToken))
                    {
                        continue;
                    }

                    var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(stakingToken.Id, market.Id));

                    if (!tokens.TryGetValue(liquidityPool.LpTokenId, out var lpToken))
                    {
                        continue;
                    }

                    foreach (var snapshotType in snapshotTypes)
                    {
                        await ProcessLiquidityPoolSnapshot(liquidityPool.Id, market.Id, stakingToken, lpToken, request.CrsUsd, snapshotType, blockTime);
                    }

                    var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingToken.Id,
                                                                                                             market.Id, blockTime,
                                                                                                             SnapshotType.Daily));
                    stakingTokenUsd = stakingTokenSnapshot.Price.Close;
                }

                // Every pool excluding the staking token and it's liquidity pool
                foreach(var liquidityPool in marketPools.Where(mp => mp.SrcTokenId != market.StakingTokenId))
                {
                    if (!tokens.TryGetValue(liquidityPool.SrcTokenId, out var srcToken) ||
                        !tokens.TryGetValue(liquidityPool.LpTokenId, out var lpToken))
                    {
                        continue;
                    }

                    foreach (var snapshotType in snapshotTypes)
                    {
                        await ProcessLiquidityPoolSnapshot(liquidityPool.Id, market.Id, srcToken, lpToken, request.CrsUsd, snapshotType, blockTime, stakingTokenUsd);
                    }
                }

                // Process market snapshot
                await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id));
            }

            return Unit.Value;
        }

        private async Task ProcessLiquidityPoolSnapshot(long liquidityPoolId, long marketId, Token srcToken, Token lpToken, decimal crsUsd,
                                                        SnapshotType snapshotType, DateTime blockTime, decimal? stakingTokenUsd = null)
        {
            // Retrieve liquidity pool snapshot
            var lpSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPoolId,
                                                                                                   blockTime,
                                                                                                   snapshotType));
            // Process new token snapshot
            var srcUsd = await _mediator.Send(new ProcessSrcTokenSnapshotCommand(marketId,
                                                                                 srcToken,
                                                                                 snapshotType,
                                                                                 blockTime,
                                                                                 crsUsd,
                                                                                 lpSnapshot.Reserves.Crs,
                                                                                 lpSnapshot.Reserves.Src));

            // When processing a liquidity pool of a staking token, use the srcUsd value instead.
            var stakingUsd = stakingTokenUsd ?? srcUsd;

            // Process latest lp snapshot
            lpSnapshot.ResetStaleSnapshot(crsUsd, srcUsd, stakingUsd, srcToken.Decimals, blockTime);

            await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(lpSnapshot));

            // Process latest lp token snapshot
            var lptUsd = await _mediator.Send(new ProcessLpTokenSnapshotCommand(marketId,
                                                                                lpToken,
                                                                                lpSnapshot.Reserves.Usd,
                                                                                snapshotType,
                                                                                blockTime));
        }
    }
}