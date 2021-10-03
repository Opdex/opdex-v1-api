using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
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
            try
            {
                // The latest synced block we have, if we don't have any, the tip of Cirrus chain, else null
                var bestBlock = await _mediator.Send(new GetBestBlockQuery(), cancellationToken);

                // Rewind when applicable
                if (bestBlock == null)
                {
                    var dbLatestBlock = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: true));
                    var commonCirrusBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(dbLatestBlock.Hash, findOrThrow: false));

                    while (commonCirrusBlock == null)
                    {
                        dbLatestBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(dbLatestBlock.Height - 1));
                        commonCirrusBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(dbLatestBlock.Hash, findOrThrow: false));
                    }

                    var rewound = await _mediator.Send(new CreateRewindToBlockCommand(commonCirrusBlock.Height));
                    if (!rewound) throw new Exception($"Failure rewinding database to block height: {commonCirrusBlock.Height}");

                    bestBlock = await _mediator.Send(new GetBestBlockQuery());
                    if (bestBlock == null) throw new Exception("Rewound database and still cannot find matching best block.");
                }

                // Process each block until we reach the chain tip
                while (bestBlock.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
                {
                    // Retrieve and create the block
                    var currentBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(bestBlock.NextBlockHash, findOrThrow: true));
                    var blockCreated = await _mediator.Send(new CreateBlockCommand(currentBlock));

                    if (!blockCreated) break;

                    if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime))
                    {
                        // Dev Environment = 15 minutes, otherwise 1 minute
                        if (request.NetworkType != NetworkType.DEVNET || currentBlock.MedianTime.Minute % 15 == 0)
                        {
                            await _mediator.Send(new CreateCrsTokenSnapshotsCommand(currentBlock.MedianTime));
                        }
                    }

                    var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus));

                    var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, currentBlock.MedianTime, SnapshotType.Minute));

                    // If it's a new day from the previous block, refresh all daily snapshots. (Tokens, Liquidity Pools, Markets)
                    if (currentBlock.IsNewDayFromPrevious(bestBlock.MedianTime))
                    {
                        await _mediator.Send(new ProcessDailySnapshotRefreshCommand(currentBlock.Height, currentBlock.MedianTime, crsSnapshot.Price.Close));
                    }

                    // Process all transactions in the block
                    foreach (var tx in currentBlock.TxHashes.Where(tx => tx != currentBlock.MerkleRoot))
                    {
                        // Todo: Consider processing liquidity pool snapshots after each block rather than during each transaction.
                        await _mediator.Send(new CreateTransactionCommand(tx));
                    }

                    // Process market snapshots every 5 minutes
                    if (currentBlock.IsNewMinuteFromPrevious(bestBlock.MedianTime) &&
                        currentBlock.MedianTime.Minute % 5 == 0)
                    {
                        var markets = await _mediator.Send(new RetrieveAllMarketsQuery());

                        foreach (var market in markets)
                        {
                            await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id, currentBlock.MedianTime));
                        }
                    }

                    bestBlock = currentBlock;
                }
            }
            // Not Found exception may not be specific enough, might want a unique exception related to not found block.
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Unable to find best block, attempting rewind");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure processing blocks");
            }

            return Unit.Value;
        }
    }
}
