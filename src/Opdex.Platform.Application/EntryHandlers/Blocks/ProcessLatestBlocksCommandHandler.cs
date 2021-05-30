using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;

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
            var blockDetails = await _mediator.Send(new GetBestBlockQuery(), cancellationToken);

            while (blockDetails?.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Todo: Move through Domain, at least a new CirrusBlock model
                // Todo: Get block details from cirrus node and filter for nonstandard transactions
                blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockDetails.NextBlockHash), CancellationToken.None);

                var createBlockCommand = new CreateBlockCommand(blockDetails.Height, blockDetails.Hash, blockDetails.Time, blockDetails.MedianTime);
                var blockCreated = await _mediator.Send(createBlockCommand, CancellationToken.None);

                if (!blockCreated)
                {
                    break;
                }

                // 4 = 1 minute || 60 = 15 minutes
                var timeToRefreshCirrus = request.IsDevelopEnv ? 60ul : 4ul;

                if (blockDetails.Height % timeToRefreshCirrus == 0)
                {
                    await _mediator.Send(new CreateCrsTokenSnapshotsCommand(createBlockCommand.MedianTime), CancellationToken.None);

                    // Todo should also snapshot ODX Token if there is a staking market available
                }

                // Index each transaction in the block
                foreach (var tx in blockDetails.Tx.Where(tx => tx != blockDetails.MerkleRoot))
                {
                    try
                    {
                        await _mediator.Send(new CreateTransactionCommand(tx), CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Unable to create transaction with error: {ex.Message}");
                    }
                }

                // Maybe create liquidity pool snapshots after each block
                // Maybe create mining pool snapshots after each block
                // Index Market Snapshots based on Pool Snapshots in time tx time range
            }

            return Unit.Value;
        }
    }
}