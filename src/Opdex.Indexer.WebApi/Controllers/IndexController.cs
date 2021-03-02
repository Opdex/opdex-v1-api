using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Indexer.Application.Abstractions.EntryCommands.Transactions;
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
            return Ok();
        }

        /// <summary>
        /// Indexes current up to latest block and publishes an integration event for each new transaction
        /// </summary>
        [HttpPost("process-blocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ProcessBlocks()
        {
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