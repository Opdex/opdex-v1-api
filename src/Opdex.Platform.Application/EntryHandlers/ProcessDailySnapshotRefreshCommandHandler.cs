using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

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
                var marketPools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(market.Id));
                var stakingTokenUsd = 0m;

                // Process staking tokens and their liquidity pools first
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
                        await _mediator.Send(new ProcessDailyLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, market.Id, stakingToken,
                                                                                                 lpToken, request.CrsUsd, snapshotType, blockTime));
                    }

                    var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(stakingToken.Id, market.Id,
                                                                                                             blockTime, SnapshotType.Daily));
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
                        await _mediator.Send(new ProcessDailyLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, market.Id, srcToken, lpToken,
                                                                                                 request.CrsUsd, snapshotType, blockTime, stakingTokenUsd));
                    }
                }

                // Process market snapshot
                await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, blockTime));
            }

            return Unit.Value;
        }
    }
}
