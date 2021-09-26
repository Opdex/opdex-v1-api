using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers
{
    public class CreateRewindSnapshotsCommandHandler : IRequestHandler<CreateRewindSnapshotsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindSnapshotsCommandHandler> _logger;

        public CreateRewindSnapshotsCommandHandler(IMediator mediator, ILogger<CreateRewindSnapshotsCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var rewindBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(request.RewindHeight));
            var startOfDay = rewindBlock.MedianTime.ToStartOf(SnapshotType.Daily);
            var endOfDay = rewindBlock.MedianTime.ToEndOf(SnapshotType.Daily);

            // Get All Markets
            var markets = await _mediator.Send(new RetrieveAllMarketsQuery());
            foreach (var market in markets)
            {
                // Get All Pools
                var pools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(market.Id));
                var poolsSnapshots = new List<LiquidityPoolSnapshot>();

                foreach (var pool in pools)
                {
                    // Rewind pool daily snapshot
                    var poolDailySnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, startOfDay, SnapshotType.Daily));
                    var poolHourlySnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, startOfDay, endOfDay, SnapshotType.Hourly));
                    poolDailySnapshot.RewindSnapshot(poolHourlySnapshots.ToList());
                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolDailySnapshot));
                    poolsSnapshots.Add(poolDailySnapshot);

                    // Rewind token daily snapshots
                    await RewindTokenDailySnapshot(pool.SrcTokenId, market.Id, startOfDay, endOfDay);
                    await RewindTokenDailySnapshot(pool.LpTokenId, market.Id, startOfDay, endOfDay);
                }

                // Todo: Not necessarily needed, ProcessMarketSnapshotCommand resets and recounts everyhing, this can be removed
                // and done at the end of the entire rewind, with one exception, if the rewind spans multiple days, this needs to be
                // taken into account.
                // Update the market snapshot using all the rewound daily liquidity pool snapshots
                var marketDailySnapshot = await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(market.Id, startOfDay, SnapshotType.Daily));
                marketDailySnapshot.RewindSnapshot(poolsSnapshots);
                await _mediator.Send(new MakeMarketSnapshotCommand(marketDailySnapshot));
            }

            // Todo: Sort Order Transactions
            // Todo: Verify Sort Order Transaction Logs
            // Get all transactions using RewindHeight
            //     - Retrieved Transactions must be Transaction Domain models and include logs
            //     - Retrieved Transactions must be from the start of the hour of the RewindHeight time
            var transactions = new List<Transaction>();

            // Group transactions by block - loop
            var transactionsByBlock = transactions.GroupBy(t => t.BlockHeight);
            foreach (var transactionBlockGroup in transactionsByBlock)
            {
                // Todo: Might not need to group by block if we reuse this much functionality.
                // Group by block and create custom rewind methods for improved performance, going with reuse for now.
                var block = transactionBlockGroup.Key;
                foreach (var transaction in transactionBlockGroup)
                {
                    // Rebuild all snapshots based on the beginning of the hour from this Rewind Block
                    await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));
                }
            }

            return true;
        }

        private async Task RewindTokenDailySnapshot(long tokenId, long marketId, DateTime startOfDay, DateTime endOfDay)
        {
            var srcTokenDailySnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(tokenId, marketId, startOfDay, SnapshotType.Daily));
            var srcTokenHourlySnapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(tokenId, marketId, startOfDay, endOfDay, SnapshotType.Hourly));
            srcTokenDailySnapshot.RewindSnapshot(srcTokenHourlySnapshots.ToList());
            await _mediator.Send(new MakeTokenSnapshotCommand(srcTokenDailySnapshot));
        }
    }
}
