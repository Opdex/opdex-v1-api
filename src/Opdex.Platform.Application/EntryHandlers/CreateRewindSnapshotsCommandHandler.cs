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
                    poolDailySnapshot.RewindSnapshot(poolHourlySnapshots.ToList());
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

            // Get transactions
            var transactions = await _mediator.Send(new SelectTransactionsForSnapshotRewindQuery(startOfHour));
            var transactionsList = transactions.ToList();

            _logger.LogDebug($"Found {transactionsList.Count} transactions to reprocess liquidity pool and token snapshot history.");

            foreach (var transaction in transactionsList)
                await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));

            _logger.LogDebug($"Replayed {transactionsList.Count} transactions to refresh liquidity pool and token snapshot history.");

            _logger.LogDebug($"Rebuilding {marketList.Count} stale markets snapshots.");

            // Process market daily snapshots with all of the latest rewind data
            foreach (var market in marketList)
                await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, startOfDay));

            _logger.LogDebug($"Refreshed {marketList.Count} stale markets snapshots.");

            return true;
        }

        private async Task<bool> RewindTokenDailySnapshot(long tokenId, long marketId, DateTime startOfDay, DateTime endOfDay)
        {
            var srcTokenDailySnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(tokenId, marketId, startOfDay, SnapshotType.Daily));
            var srcTokenHourlySnapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(tokenId, marketId, startOfDay, endOfDay, SnapshotType.Hourly));
            srcTokenDailySnapshot.RewindSnapshot(srcTokenHourlySnapshots.ToList());
            return await _mediator.Send(new MakeTokenSnapshotCommand(srcTokenDailySnapshot));
        }
    }
}
