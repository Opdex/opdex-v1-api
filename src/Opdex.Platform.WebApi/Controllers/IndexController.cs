using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public IndexController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("last-synced-block")]
        public async Task<IActionResult> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
            return Ok(latestSyncedBlock);
        }
        
        [HttpPost("process-latest-blocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ProcessLatestBlocks()
        {
            // Todo: Use Get Query
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
            var blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestSyncedBlock.Hash));

            while (blockDetails?.NextBlockHash != null)
            {
                // Todo: Move through Domain, at least a new CirrusBlock model
                // Todo: Probably need to get all blockDetails and filter for nonstandard transactions
                blockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(blockDetails.NextBlockHash));

                // Todo: Use CreateBlockCommand
                var blockCommand = new MakeBlockCommand(blockDetails.Height, blockDetails.Hash,
                    blockDetails.Time.FromUnixTimeSeconds(), blockDetails.MedianTime.FromUnixTimeSeconds());
                
                var createdBlock = await _mediator.Send(blockCommand);
                
                if (!createdBlock) return NoContent();

                // Once a minute sync CRS price
                if (blockDetails.Height % 4 == 0)
                {
                    await _mediator.Send(new CreateCrsTokenSnapshotsCommand(blockDetails.MedianTime.FromUnixTimeSeconds()));
                }
                
                foreach (var tx in blockDetails.Tx.Where(tx => tx != blockDetails.MerkleRoot))
                {
                    try
                    {
                        // Todo: return out transactionDto or Id to get by
                        await _mediator.Send(new CreateTransactionCommand(tx));
                    }
                    catch (Exception ex)
                    {
                        // Errrmmmm
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