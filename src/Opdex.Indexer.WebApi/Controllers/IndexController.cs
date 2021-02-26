using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Todo: Auth Attribute - InternalAccessOnly
        [HttpPut("reindex")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReIndexBlocks(ReIndexRequest request, CancellationToken cancellationToken)
        {
            return NoContent();
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
            return NoContent();
        }
    }
}