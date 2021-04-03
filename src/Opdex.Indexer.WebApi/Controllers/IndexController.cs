using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Indexer.Application.Abstractions.Commands.Blocks;
using Opdex.Indexer.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Blocks;
using Opdex.Indexer.WebApi.Models;

namespace Opdex.Indexer.WebApi.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
            return Ok(latestSyncedBlock);
        }

        /// <summary>
        /// Indexes current up to latest block and publishes an integration event for each new transaction
        /// </summary>
        [HttpPost("process-latest-blocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ProcessLatestBlocks()
        {
            // Todo: Add Get Query
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
            var cirrusBlockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestSyncedBlock.Hash));

            while (cirrusBlockDetails?.NextBlockHash != null)
            {
                // Todo: Move through Domain
                cirrusBlockDetails = await _mediator.Send(new RetrieveCirrusBlockByHashQuery(cirrusBlockDetails.NextBlockHash));

                long.TryParse(cirrusBlockDetails.Time, out var timeSeconds);
                long.TryParse(cirrusBlockDetails.MedianTime, out var medianTimeSeconds);
                var time = DateTimeOffset.FromUnixTimeSeconds(timeSeconds).UtcDateTime;
                var medianTime = DateTimeOffset.FromUnixTimeSeconds(medianTimeSeconds).UtcDateTime;
                
                // Todo: Use Crate Query
                var createdBlock = await _mediator.Send(new MakeBlockCommand(cirrusBlockDetails.Height, cirrusBlockDetails.Hash, time, medianTime));
                if (!createdBlock) return NoContent();
                
                foreach (var tx in cirrusBlockDetails.Tx.Where(tx => tx != cirrusBlockDetails.Hash))
                {
                    try
                    {
                        await _mediator.Send(new CreateTransactionCommand(tx));
                    }
                    catch (Exception ex)
                    {
                        // Errrmmmm
                    }
                }
            }
            
            // Query DB - Get latest synced block
            // Call Cirrus Get Block
            // while nextBlockHash != null - Get next block
            //    - Publish message for every transaction in block
            //    - Index block related data
            
            // Shortcut, if Block ends in "1" or "6", process snapshot for the previous 5 blocks
            // (e.g. block number is 16, process snapshots of blocks 11-15)
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

        [HttpPost("process-market")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessMarket()
        {
            return NoContent();
        }
    }
}