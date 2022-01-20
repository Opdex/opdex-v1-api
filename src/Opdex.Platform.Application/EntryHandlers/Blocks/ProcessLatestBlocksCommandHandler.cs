using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

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
                var currentBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(bestBlock.NextBlockHash.Value,
                                                                                                  findOrThrow: true), CancellationToken.None);
                var blockCreated = await _mediator.Send(new CreateBlockCommand(currentBlock), CancellationToken.None);

                if (!blockCreated) break;

                if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime))
                {
                    // Dev Environment = 15 minutes, otherwise 1 minute
                    if (request.NetworkType != NetworkType.DEVNET || currentBlock.MedianTime.Minute % 15 == 0)
                    {
                        var snapshotsCreated = await _mediator.Send(new CreateCrsTokenSnapshotsCommand(currentBlock.MedianTime,
                                                                                                       currentBlock.Height), CancellationToken.None);
                        if (!snapshotsCreated) break;
                    }
                }

                decimal? crsUsd = null;

                // If it's a new day from the previous block, refresh all daily snapshots. (Tokens, Liquidity Pools, Markets)
                if (currentBlock.IsNewDayFromPrevious(bestBlock.MedianTime))
                {
                    crsUsd = await GetCrsUsd(currentBlock.MedianTime);

                    await _mediator.Send(new ProcessDailySnapshotRefreshCommand(currentBlock.Height, currentBlock.MedianTime,
                                                                                crsUsd.Value), CancellationToken.None);
                }

                // Update stale snapshots (every 5 blocks) by block threshold (50 blocks)
                if (currentBlock.Height % 5 == 0)
                {
                    crsUsd ??= await GetCrsUsd(currentBlock.MedianTime);

                    await _mediator.Send(new ProcessStaleLiquidityPoolSnapshotsCommand(currentBlock.Height, currentBlock.Time,
                                                                                       crsUsd.Value), CancellationToken.None);
                }

                // Process all transactions in the block
                foreach (var tx in currentBlock.TxHashes.Where(tx => tx != currentBlock.MerkleRoot))
                {
                    await _mediator.Send(new CreateTransactionCommand(tx), CancellationToken.None);
                }

                // Process market snapshots every 2 minutes
                if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime) && currentBlock.MedianTime.Minute % 2 == 0)
                {
                    var markets = await _mediator.Send(new RetrieveAllMarketsQuery(), CancellationToken.None);

                    foreach (var market in markets)
                    {
                        await _mediator.Send(new ProcessMarketSnapshotsCommand(market, currentBlock.MedianTime, currentBlock.Height), CancellationToken.None);
                    }
                }

                bestBlock = currentBlock;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure processing blocks");
        }

        return Unit.Value;
    }

    private async Task<decimal> GetCrsUsd(DateTime blockTime)
    {
        var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus), CancellationToken.None);

        // Use Snapshots rather than summary in case it is a rewind to select price at specific point in time
        var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, blockTime,
                                                                                        SnapshotType.Minute), CancellationToken.None);

        return crsSnapshot.Price.Close;
    }
}
