using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Snapshots
{
    public class ProcessMarketSnapshotsCommandHandler : IRequestHandler<ProcessMarketSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

        public ProcessMarketSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessMarketSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var marketPools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(new LiquidityPoolsCursor(request.Market.Address)));
            var marketSnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(request.Market.Id, request.BlockTime, SnapshotType));
            var stakingTokenUsdStartOfDay = 0m;

            if (request.Market.IsStakingMarket)
            {
                var stakingTokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(request.Market.StakingTokenId,
                                                                                                         request.Market.Id,
                                                                                                         request.BlockTime,
                                                                                                         SnapshotType));

                stakingTokenUsdStartOfDay = stakingTokenSnapshot.Price.Open;
            }

            // Reset stale snapshot if its old
            if (marketSnapshot.EndDate < request.BlockTime)
            {
                marketSnapshot.ResetStaleSnapshot(request.BlockTime, stakingTokenUsdStartOfDay);
            }

            var poolChunks = marketPools.Chunk(20);
            var snapshots = new List<LiquidityPoolSnapshot>();

            foreach (var chunk in poolChunks)
            {
                var chunkSnapshots = await Task.WhenAll(chunk.Select(async pool =>
                {
                    return await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, request.BlockTime, SnapshotType));
                }));

                snapshots.AddRange(chunkSnapshots);
            }

            // Apply LP snapshot to market snapshot
            marketSnapshot.Update(snapshots);

            // Persist market snapshot
            await _mediator.Send(new MakeMarketSnapshotCommand(marketSnapshot));

            return Unit.Value;
        }
    }
}
