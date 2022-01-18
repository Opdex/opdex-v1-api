using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Snapshots;

public class ProcessStaleLiquidityPoolSnapshotsCommandHandler : IRequestHandler<ProcessStaleLiquidityPoolSnapshotsCommand, Unit>
{
    private readonly IMediator _mediator;

    public ProcessStaleLiquidityPoolSnapshotsCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<Unit> Handle(ProcessStaleLiquidityPoolSnapshotsCommand request, CancellationToken cancellationToken)
    {
        var snapshotTypes = new[] { SnapshotType.Hourly, SnapshotType.Daily };

        var liquidityPools = await _mediator.Send(new RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(50), CancellationToken.None);

        var poolsByMarket = liquidityPools.GroupBy(k => k.MarketId);

        foreach (var marketGroup in poolsByMarket)
        {
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(marketGroup.Key), CancellationToken.None);
            var stakingTokenUsd = 0m;

            if (market.IsStakingMarket)
            {
                var liquidityPool = marketGroup.SingleOrDefault(pool => pool.SrcTokenId == market.StakingTokenId);

                // If its not null, then its a stale snapshot, refresh it first
                if (liquidityPool != null)
                {
                    var stakingToken = await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId), CancellationToken.None);
                    var lpToken = await _mediator.Send(new RetrieveTokenByAddressQuery(liquidityPool.Address), CancellationToken.None);

                    foreach (var snapshotType in snapshotTypes)
                    {
                        await _mediator.Send(new ProcessDailyLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, liquidityPool.MarketId, stakingToken,
                                                                                                 lpToken, request.CrsUsd, snapshotType, request.BlockTime,
                                                                                                 request.BlockHeight));
                    }
                }

                var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(market.StakingTokenId, market.Id, request.BlockTime,
                                                                                                         SnapshotType.Hourly), CancellationToken.None);
                stakingTokenUsd = stakingTokenSnapshot.Price.Close;
            }

            foreach (var liquidityPool in marketGroup.Where(pool => pool.SrcTokenId != market.StakingTokenId))
            {
                var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId), CancellationToken.None);
                var lpToken = await _mediator.Send(new RetrieveTokenByAddressQuery(liquidityPool.Address), CancellationToken.None);

                foreach (var snapshotType in snapshotTypes)
                {
                    await _mediator.Send(new ProcessDailyLiquidityPoolSnapshotRefreshCommand(liquidityPool.Id, liquidityPool.MarketId, srcToken, lpToken,
                                                                                             request.CrsUsd, snapshotType, request.BlockTime,
                                                                                             request.BlockHeight, stakingTokenUsd));

                }
            }
        }

        return Unit.Value;
    }
}
