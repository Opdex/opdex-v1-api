using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
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

        // The RewindToBlock stored procedure deletes liquidity pool and token snapshots down to the hour, does not delete daily snapshots of the block being rewound to
        // It deletes market snapshots to the day of the rewind block, so a new daily market snapshot will need to be recreated after rewinding pools.
        // First we must rewind all liquidity pool and token snapshots by using the existing hourly snapshots.
        // Next we must get all transactions from the start of the hour of the rewind block. (If rewind block is at 1:35 - we need all transactions from 1:00)
        // Using found transactions, reprocess liquidity pool and token snapshots as we would normally which will pick up where we left off
        // Now Liquidity pool and token snapshots are in their latest state, use liquidity pool snapshots to refresh the daily market snapshots
        public async Task<bool> Handle(CreateRewindSnapshotsCommand request, CancellationToken cancellationToken)
        {
            var rewindBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(request.RewindHeight));
            var startOfDay = rewindBlock.MedianTime.ToStartOf(SnapshotType.Daily);
            var startOfHour = rewindBlock.MedianTime.ToStartOf(SnapshotType.Hourly);
            var endOfDay = rewindBlock.MedianTime.ToEndOf(SnapshotType.Daily);

            // Get All Markets
            var markets = await _mediator.Send(new RetrieveAllMarketsQuery());
            var marketList = markets.ToList();

            _logger.LogDebug($"Found {marketList.Count} stale markets.");

            // Every pool and every token need their Daily snapshots re-calculated up to the rewind hour
            foreach (var market in marketList)
            {
                // Get All Pools
                var pools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(market.Id));
                var poolsList = pools.ToList();

                _logger.LogDebug($"Found {poolsList.Count} stale liquidity pool daily snapshots.");
                _logger.LogDebug($"Found {poolsList.Count * 2} stale token daily snapshots.");

                int poolRefreshFailures = 0;
                int tokenRefreshFailures = 0;

                foreach (var pool in poolsList)
                {
                    // Rewind pool daily snapshot
                    var poolDailySnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, startOfDay, SnapshotType.Daily));
                    var poolHourlySnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, startOfDay, endOfDay, SnapshotType.Hourly));
                    poolDailySnapshot.RewindDailySnapshot(poolHourlySnapshots.ToList());
                    var resetPoolSnapshot = await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolDailySnapshot));
                    if (!resetPoolSnapshot) poolRefreshFailures++;

                    // Rewind token daily snapshots
                    var srcSnapshot = await RewindTokenDailySnapshot(pool.SrcTokenId, market.Id, startOfDay, endOfDay);
                    if (!srcSnapshot) tokenRefreshFailures++;

                    var lptSnapshot = await RewindTokenDailySnapshot(pool.LpTokenId, market.Id, startOfDay, endOfDay);
                    if (!lptSnapshot) tokenRefreshFailures++;
                }

                _logger.LogDebug($"Rewound {poolsList.Count - poolRefreshFailures} stale liquidity pool daily snapshots.");
                _logger.LogDebug($"Rewound {(poolsList.Count * 2) - tokenRefreshFailures} stale token daily snapshots.");

                if (poolRefreshFailures > 0) _logger.LogError($"Failed to reset {poolRefreshFailures} stale liquidity pools daily snapshots.");
                if (tokenRefreshFailures > 0) _logger.LogError($"Failed to reset {tokenRefreshFailures} stale token daily snapshots.");
            }

            // Get transactions from the beginning of the rewind block hour
            var transactions = await _mediator.Send(new SelectTransactionsForSnapshotRewindQuery(startOfHour));
            var transactionsList = transactions.ToList();

            // Using transactions from the beginning of the hour, re-process related snapshots
            _logger.LogDebug($"Found {transactionsList.Count} transactions to reprocess liquidity pool and token snapshot history.");
            foreach (var transaction in transactionsList)
            {
                await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));
            }
            _logger.LogDebug($"Replayed {transactionsList.Count} transactions to refresh liquidity pool and token snapshot history.");

            // Process market daily snapshots with all of the latest rewind data
            _logger.LogDebug($"Rebuilding {marketList.Count} stale markets snapshots.");
            foreach (var market in marketList)
            {
                await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, startOfDay));
            }
            _logger.LogDebug($"Refreshed {marketList.Count} stale markets snapshots.");

            return true;
        }

        private async Task<bool> RewindTokenDailySnapshot(long tokenId, long marketId, DateTime startOfDay, DateTime endOfDay)
        {
            var srcTokenDailySnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(tokenId, marketId, startOfDay, SnapshotType.Daily));
            var srcTokenHourlySnapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(tokenId, marketId, startOfDay, endOfDay, SnapshotType.Hourly));
            srcTokenDailySnapshot.RewindDailySnapshot(srcTokenHourlySnapshots.ToList());
            return await _mediator.Send(new MakeTokenSnapshotCommand(srcTokenDailySnapshot));
        }
    }
}
