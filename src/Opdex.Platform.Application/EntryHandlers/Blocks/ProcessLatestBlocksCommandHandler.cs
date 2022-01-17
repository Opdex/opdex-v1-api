using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class ProcessLatestBlocksCommandHandler : IRequestHandler<ProcessLatestBlocksCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessLatestBlocksCommandHandler> _logger;
    public ProcessLatestBlocksCommandHandler(IMediator mediator, ILogger<ProcessLatestBlocksCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(ProcessLatestBlocksCommand request, CancellationToken cancellationToken)
    {
        var bestBlock = request.CurrentBlock;

        try
        {
            // Process each block until we reach the chain tip
            while (bestBlock.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Retrieve and create the block
                var currentBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(bestBlock.NextBlockHash.Value, findOrThrow: true), CancellationToken.None);
                var blockCreated = await _mediator.Send(new CreateBlockCommand(currentBlock), CancellationToken.None);

                if (!blockCreated) break;

                if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime))
                {
                    // Dev Environment = 15 minutes, otherwise 1 minute
                    if (request.NetworkType != NetworkType.DEVNET || currentBlock.MedianTime.Minute % 15 == 0)
                    {
                        var snapshotsCreated = await _mediator.Send(new CreateCrsTokenSnapshotsCommand(currentBlock.MedianTime, currentBlock.Height), CancellationToken.None);
                        if (!snapshotsCreated) break;
                    }
                }

                var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus), CancellationToken.None);

                var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, currentBlock.MedianTime, SnapshotType.Minute), CancellationToken.None);

                // If it's a new day from the previous block, refresh all daily snapshots. (Tokens, Liquidity Pools, Markets)
                if (currentBlock.IsNewDayFromPrevious(bestBlock.MedianTime))
                {
                    await _mediator.Send(new ProcessDailySnapshotRefreshCommand(currentBlock.Height, currentBlock.MedianTime, crsSnapshot.Price.Close), CancellationToken.None);
                }

                // Process all transactions in the block
                foreach (var tx in currentBlock.TxHashes.Where(tx => tx != currentBlock.MerkleRoot))
                {
                    // Todo: Consider processing liquidity pool snapshots after each block rather than during each transaction.
                    await _mediator.Send(new CreateTransactionCommand(tx), CancellationToken.None);
                }

                // Process market snapshots every 2 minutes
                if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime) &&
                    currentBlock.MedianTime.Minute % 2 == 0)
                {
                    var markets = await _mediator.Send(new RetrieveAllMarketsQuery(), CancellationToken.None);

                    foreach (var market in markets)
                    {
                        await _mediator.Send(new ProcessMarketSnapshotsCommand(market, currentBlock.MedianTime, currentBlock.Height), CancellationToken.None);
                    }
                }

                bestBlock = currentBlock;
            }

            // Once blocks are fully processed, update stale liquidity pool snapshots and tokens
            var staleSnapshots = await _mediator.Send(new RetrieveStaleLiquidityPoolSnapshotsQuery(), CancellationToken.None);
            var snapshotsByPool = staleSnapshots.GroupBy(k => k.LiquidityPoolId);

            foreach (var poolSnapshots in snapshotsByPool)
            {
                // Todo: Need to get market and prioritize staking token liquidity pool updates first
                // After need to get latest staking token usd from sharable location for each snapshot to reuse.

                var liquidityPoolId = poolSnapshots.Key;
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(liquidityPoolId), CancellationToken.None);
                var snapshotTypes = new List<SnapshotType> { SnapshotType.Hourly, SnapshotType.Daily };

                // If daily is returned and not hourly
                // -- There is no hourly, create one

                // If hourly is returned and not daily
                // -- shouldn't happen, throw / handle

                // If daily + hourly
                // -- Refresh both

                foreach (var snapshotType in snapshotTypes)
                {
                    var snapshot = poolSnapshots.FirstOrDefault(snapshot => snapshot.SnapshotType == snapshotType);
                    if (snapshot == null) snapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(liquidityPoolId,
                                                                                                                           DateTime.UtcNow,
                                                                                                                           snapshotType));

                    var crsUsd = 0;
                    var stakingTokenUsd = 0;
                    snapshot.RefreshSnapshotFiatAmounts(crsUsd, stakingTokenUsd);

                    await _mediator.Send(new MakeLiquidityPoolSnapshotCommand(snapshot, bestBlock.Height), CancellationToken.None);

                    // Process Pool SRC Token Snapshot
                    // Process Pool LP Token Snapshot
                }

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure processing blocks");
        }

        return Unit.Value;
    }
}
