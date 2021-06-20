using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;

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

        // Todo: Forks and Chain Reorgs :(
        public async Task<Unit> Handle(ProcessLatestBlocksCommand request, CancellationToken cancellationToken)
        {
            // The latest synced block we have, if none, the tip of cirrus chain
            var previousBlock = await _mediator.Send(new GetBestBlockQuery(), cancellationToken);

            // Process each block until we reach the chain tip
            while (previousBlock?.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Retrieve and create the block
                var currentBlock = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(previousBlock.NextBlockHash), CancellationToken.None);
                var blockCreated = await _mediator.Send(new CreateBlockCommand(currentBlock), CancellationToken.None);

                // Stop if the block wasn't created
                if (!blockCreated) break;

                // Every minute outside of DEV, refresh CRS USD prices
                if (currentBlock.IsNewMinuteFromPrevious(previousBlock.MedianTime))
                {
                    // Dev Environment = 15 minutes, otherwise 1 minute
                    if (!request.IsDevelopEnv || currentBlock.MedianTime.Minute == 15)
                    {
                        await _mediator.Send(new CreateCrsTokenSnapshotsCommand(currentBlock.MedianTime), CancellationToken.None);
                    }
                }

                // Get CRS Token and it's snapshot at or prior to this block
                var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));
                var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, currentBlock.MedianTime, SnapshotType.Minute));

                // If it's a new day from the previous block, refresh all daily snapshots. (Tokens, Liquidity Pools, Markets)
                if (currentBlock.IsNewDayFromPrevious(previousBlock.MedianTime))
                {
                    await _mediator.Send(new ProcessDailySnapshotRefreshCommand(currentBlock.MedianTime, crsSnapshot.Price.Close));
                }

                // Process all transactions in the block
                foreach (var tx in currentBlock.TxHashes.Where(tx => tx != currentBlock.MerkleRoot))
                {
                    await _mediator.Send(new CreateTransactionCommand(tx), CancellationToken.None);
                }

                // Todo: Implement or remove blow Consideration comment
                // Consideration: process liquidity pool snapshots after each block rather than during each transaction.

                // Get and process all available Opdex markets
                // Todo: Don't fetch and process all markets, only those that had transactions in the block being processed.
                var markets = await _mediator.Send(new RetrieveAllMarketsQuery());

                foreach (var market in markets)
                {
                    await _mediator.Send(new ProcessMarketSnapshotsCommand(market.Id));
                }

                previousBlock = currentBlock;
            }

            return Unit.Value;
        }
    }
}