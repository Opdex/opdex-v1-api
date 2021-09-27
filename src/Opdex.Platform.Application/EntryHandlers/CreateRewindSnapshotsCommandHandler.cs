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

            // Every pool and every token need their Daily snapshots re-calculated up to the rewind hour
            foreach (var market in marketList)
            {
                // Get All Pools
                var pools = await _mediator.Send(new RetrieveLiquidityPoolsWithFilterQuery(market.Id));

                foreach (var pool in pools)
                {
                    // Rewind pool daily snapshot
                    var poolDailySnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, startOfDay, SnapshotType.Daily));
                    var poolHourlySnapshots = await _mediator.Send(new RetrieveLiquidityPoolSnapshotsWithFilterQuery(pool.Id, startOfDay, endOfDay, SnapshotType.Hourly));
                    poolDailySnapshot.RewindSnapshot(poolHourlySnapshots.ToList());
                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(poolDailySnapshot));

                    // Rewind token daily snapshots
                    await RewindTokenDailySnapshot(pool.SrcTokenId, market.Id, startOfDay, endOfDay);
                    await RewindTokenDailySnapshot(pool.LpTokenId, market.Id, startOfDay, endOfDay);
                }
            }

            // Get transactions
            var transactions = await _mediator.Send(new SelectTransactionsForSnapshotRewindQuery(startOfHour));
            foreach (var transaction in transactions)
                await _mediator.Send(new ProcessLiquidityPoolSnapshotsByTransactionCommand(transaction));

            // Process market daily snapshots with all of the latest rewind data
            foreach (var market in marketList)
                await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, startOfDay));

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
