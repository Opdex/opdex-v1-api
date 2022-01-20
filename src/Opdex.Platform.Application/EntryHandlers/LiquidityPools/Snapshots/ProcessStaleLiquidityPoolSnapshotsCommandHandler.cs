using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;

public class ProcessStaleLiquidityPoolSnapshotsCommandHandler : IRequestHandler<ProcessStaleLiquidityPoolSnapshotsCommand, Unit>
{
    private readonly IMediator _mediator;
    private static readonly SnapshotType[] SnapshotTypes = { SnapshotType.Hourly, SnapshotType.Daily };
    private static readonly ParallelOptions PoolParallelOptions = new() { MaxDegreeOfParallelism = 3 };

    public ProcessStaleLiquidityPoolSnapshotsCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Unit> Handle(ProcessStaleLiquidityPoolSnapshotsCommand request, CancellationToken cancellationToken)
    {
        var staleLiquidityPools = await _mediator.Send(new RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(50), CancellationToken.None);

        var poolsByMarket = staleLiquidityPools.GroupBy(k => k.MarketId);

        foreach (var marketGroup in poolsByMarket)
        {
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(marketGroup.Key), CancellationToken.None);
            var stakingTokenUsd = 0m;

            if (market.IsStakingMarket)
            {
                // Look to see if the market's staking token is stale
                var stakingTokenPool = marketGroup.SingleOrDefault(pool => pool.SrcTokenId == market.StakingTokenId);

                // Only refresh the staking token's pool if its stale
                if (stakingTokenPool != null)
                {
                    await ProcessLiquidityPoolSnapshotRefresh(stakingTokenPool, request.CrsUsd, request.BlockTime, request.BlockHeight, stakingTokenUsd);
                }

                // Retrieve and return the market's staking token USD price
                var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(market.StakingTokenId, market.Id, request.BlockTime,
                                                                                                         SnapshotType.Hourly), CancellationToken.None);
                stakingTokenUsd = stakingTokenSnapshot.Price.Close;
            }

            // Filter out the market's staking token's pool, taken care above
            var liquidityPools = marketGroup.Where(pool => pool.SrcTokenId != market.StakingTokenId);

            // Process all stale liquidity pools in the market, in groups of the amount configured in parallel options.
            await Parallel.ForEachAsync(liquidityPools, PoolParallelOptions, async (liquidityPool, _) =>
            {
                await ProcessLiquidityPoolSnapshotRefresh(liquidityPool, request.CrsUsd, request.BlockTime, request.BlockHeight, stakingTokenUsd);
            });
        }

        return Unit.Value;
    }

    private async Task ProcessLiquidityPoolSnapshotRefresh(LiquidityPool liquidityPool, decimal crsUsd, DateTime blockTime,
                                                           ulong blockHeight, decimal stakingTokenUsd)
    {
        var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId), CancellationToken.None);
        var lpToken = await _mediator.Send(new RetrieveTokenByAddressQuery(liquidityPool.Address), CancellationToken.None);

        // We may be inside another Parallel.ForEachAsync, probably shouldn't run another
        await Task.WhenAll(SnapshotTypes.Select(snapshotType =>
        {
            return _mediator.Send(new ProcessLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, liquidityPool.MarketId, srcToken, lpToken,
                                                                                 crsUsd, snapshotType, blockTime,
                                                                                 blockHeight, stakingTokenUsd), CancellationToken.None);
        }));
    }
}
