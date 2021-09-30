using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
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

        // The RewindToBlock stored procedure deletes liquidity pool and token snapshots down to the hour, does not delete daily snapshots of the block being rewound to
        // It deletes market snapshots to the day of the rewind block, so a new daily market snapshot will need to be recreated after rewinding pools.
        // First we must rewind all daily liquidity pool and token snapshots by using the existing hourly snapshots to rebuild them.
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

            // Rewind daily liquidity pool and token snapshots
            await RewindLiquidityPoolsAndTokens(marketList, startOfDay, endOfDay);

            // Replay transactions from the start of the hour to reprocess their associated snapshots
            await ReplayTransactionsFromStartOfHour(startOfHour);

            // Process market daily snapshots with all of the latest rewind data
            await RewindMarkets(marketList, startOfDay);

            return true;
        }

        private async Task RewindLiquidityPoolsAndTokens(IList<Market> marketList, DateTime startOfDay, DateTime endOfDay)
        {
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
                    var resetPoolSnapshot = await _mediator.Send(new CreateRewindLiquidityPoolDailySnapshotCommand(pool.Id, startOfDay, endOfDay));
                    if (!resetPoolSnapshot) poolRefreshFailures++;

                    // Rewind token daily snapshots
                    var srcSnapshot = await _mediator.Send(new CreateRewindTokenDailySnapshotCommand(pool.SrcTokenId, market.Id, startOfDay, endOfDay));
                    if (!srcSnapshot) tokenRefreshFailures++;

                    var lptSnapshot = await _mediator.Send(new CreateRewindTokenDailySnapshotCommand(pool.LpTokenId, market.Id, startOfDay, endOfDay));
                    if (!lptSnapshot) tokenRefreshFailures++;
                }

                _logger.LogDebug($"Rewound {poolsList.Count - poolRefreshFailures} stale liquidity pool daily snapshots.");
                _logger.LogDebug($"Rewound {(poolsList.Count * 2) - tokenRefreshFailures} stale token daily snapshots.");

                if (poolRefreshFailures > 0) _logger.LogError($"Failed to reset {poolRefreshFailures} stale liquidity pools daily snapshots.");
                if (tokenRefreshFailures > 0) _logger.LogError($"Failed to reset {tokenRefreshFailures} stale token daily snapshots.");
            }
        }

        private async Task ReplayTransactionsFromStartOfHour(DateTime startOfHour)
        {
            // Get transactions from the beginning of the rewind block hour
            var transactions = await _mediator.Send(new SelectTransactionsForSnapshotRewindQuery(startOfHour));
            var transactionsList = transactions.ToList();
            int transactionReplayFailures = 0;

            _logger.LogDebug($"Found {transactionsList.Count} transactions to reprocess liquidity pool and token snapshot history.");

            // Using transactions from the beginning of the hour, re-process related snapshots
            foreach (var transaction in transactionsList)
            {
                try
                {
                    await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));
                }
                catch (Exception)
                {
                    transactionReplayFailures++;
                }
            }

            _logger.LogDebug($"Replayed {transactionsList.Count - transactionReplayFailures} transactions to refresh liquidity pool and token snapshot history.");

            if (transactionReplayFailures > 0) _logger.LogError($"Failed to replay {transactionReplayFailures} transactions.");
        }

        private async Task RewindMarkets(IList<Market> marketList, DateTime startOfDay)
        {
            _logger.LogDebug($"Rebuilding {marketList.Count} stale markets snapshots.");

            int marketRewindFailures = 0;

            foreach (var market in marketList)
            {
                try
                {
                    await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, startOfDay));
                }
                catch (Exception)
                {
                    marketRewindFailures++;
                }
            }

            _logger.LogDebug($"Refreshed {marketList.Count - marketRewindFailures} stale markets snapshots.");

            if (marketRewindFailures > 0) _logger.LogError($"Failed to rewind {marketRewindFailures} markets.");
        }
    }
}
