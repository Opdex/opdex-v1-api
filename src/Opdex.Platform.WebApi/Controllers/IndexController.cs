using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexController> _logger;

        public IndexController(IMediator mediator, ILogger<IndexController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            // Todo: Use Get Query
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), CancellationToken.None);
            var blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestSyncedBlock.Hash), CancellationToken.None);

            while (blockDetails?.NextBlockHash != null && !cancellationToken.IsCancellationRequested)
            {
                // Todo: Move through Domain, at least a new CirrusBlock model
                // Todo: Probably need to get all blockDetails and filter for nonstandard transactions
                blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockDetails.NextBlockHash), CancellationToken.None);

                // Todo: Use CreateBlockCommand
                var blockCommand = new MakeBlockCommand(blockDetails.Height, blockDetails.Hash,
                    blockDetails.Time.FromUnixTimeSeconds(), blockDetails.MedianTime.FromUnixTimeSeconds());

                var createdBlock = await _mediator.Send(blockCommand, CancellationToken.None);

                if (!createdBlock)
                {
                    return NoContent();
                }

                // Once a minute sync CRS price
                if (blockDetails.Height % 20 == 0)
                {
                    await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockDetails.MedianTime.FromUnixTimeSeconds()), CancellationToken.None);
                }

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
            }

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