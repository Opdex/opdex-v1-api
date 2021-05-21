using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexController> _logger;
        private readonly IHostingEnvironment _hostingEnv;
        
        public IndexController(IMediator mediator, ILogger<IndexController> logger, IHostingEnvironment hostingEnv)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostingEnv = hostingEnv ?? throw new ArgumentNullException(nameof(hostingEnv));
        }

        [HttpGet("last-synced-block")]
        public async Task<IActionResult> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
            return Ok(latestSyncedBlock);
        }
        
        /// /// <summary>
        /// Processes all blocks and transactions after the most recent synced block in the DB.
        /// </summary>
        /// <remarks>
        /// Requires at least one block to be manually added to the DB. Will sync from that block
        /// forward to chain tip.
        /// </remarks>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>No Content</returns>
        [HttpPost("process-latest-blocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ProcessLatestBlocks(CancellationToken cancellationToken)
        {
            // Todo: Implement with index_lock table - prevent reindexing if another instance already is
            var blockDetails = await _mediator.Send(new GetBestBlockQuery(), cancellationToken);

            while (blockDetails?.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Todo: Move through Domain, at least a new CirrusBlock model
                // Todo: Probably need to get all blockDetails and filter for nonstandard transactions
                blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockDetails.NextBlockHash), CancellationToken.None);

                var createBlockCommand = new CreateBlockCommand(blockDetails.Height, blockDetails.Hash, blockDetails.Time, blockDetails.MedianTime);
                var blockCreated = await _mediator.Send(createBlockCommand, CancellationToken.None);
                
                if (!blockCreated)
                {
                    break;
                }

                // 4 = 1 minute || 60 = 15 minutes
                var timeToRefreshCirrus = _hostingEnv.IsDevelopment() ? 60ul : 4ul;
                
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
            
            return NoContent();
        }
        
        [HttpPost("process-odx-deployment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessOdxDeploymentTransaction(ProcessTransactionRequestModel txRequest)
        {
            await _mediator.Send(new ProcessOdxDeploymentTransactionCommand(txRequest.TxHash));

            return NoContent();
        }
        
        [HttpPost("process-deployer-deployment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessDeployerDeploymentTransaction(ProcessTransactionRequestModel txRequest)
        {
            await _mediator.Send(new ProcessDeployerDeploymentTransactionCommand(txRequest.TxHash));

            return NoContent();
        }
        
        [HttpPost("process-transaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessTransaction(ProcessTransactionRequestModel txRequest)
        {
            var response = await _mediator.Send(new CreateTransactionCommand(txRequest.TxHash));

            if (response == false)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}