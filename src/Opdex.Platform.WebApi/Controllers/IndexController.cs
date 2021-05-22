using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.WebApi.Models.Requests.Index;

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
            await _mediator.Send(new ProcessLatestBlocksCommand(_hostingEnv.IsDevelopment()), CancellationToken.None);
            
            return NoContent();
        }
        
        /// <summary>
        /// Processes the odx and market deployer transactions then syncs to chain tip.
        /// </summary>
        /// <remarks>
        /// For a successful redeployment, copy the odx and market deployer deployment transaction hashes.
        /// Then clear all non-lookup tables of any data in the database. Leaving only `_type` tables populated.
        /// </remarks>
        /// <param name="request">The odx and market deployer transaction hashes to look up.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>No Content</returns>
        [HttpPost("resync-from-deployment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResyncFromDeployment(ResyncFromDeploymentRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ProcessOdxDeploymentTransactionCommand(request.OdxDeploymentTxHash), CancellationToken.None);
            await _mediator.Send(new ProcessDeployerDeploymentTransactionCommand(request.MarketDeployerDeploymentTxHash), CancellationToken.None);
            await _mediator.Send(new ProcessLatestBlocksCommand(_hostingEnv.IsDevelopment()), CancellationToken.None);
            
            return NoContent();
        }
    }
}