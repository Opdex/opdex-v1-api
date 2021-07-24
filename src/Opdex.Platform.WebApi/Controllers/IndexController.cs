using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Requests.Index;
using System.Linq;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly NetworkType _network;

        public IndexController(IMediator mediator, OpdexConfiguration opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }


        /// <summary>
        /// Retrieve the latest synced block.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Block details</returns>
        [HttpGet("latest-block")]
        [Authorize]
        public async Task<IActionResult> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            // Todo: Get Query with ResponseModel
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
            return Ok(latestSyncedBlock);
        }

        /// <summary>
        /// Processes the odx and market deployer transactions then syncs to chain tip.
        /// </summary>
        /// <remarks>
        /// For a successful redeployment, copy the odx and market deployer deployment transaction hashes.
        /// Then clear all non-lookup tables of any data in the database. Leaving only `_type` tables populated.
        /// </remarks>
        /// <param name="request">The odx and market deployer transaction hashes to look up.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No Content</returns>
        [HttpPost("resync-from-deployment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResyncFromDeployment(ResyncFromDeploymentRequest request, CancellationToken cancellationToken)
        {
            var markets = await _mediator.Send(new RetrieveAllMarketsQuery(), cancellationToken);
            if (markets.Any())
            {
                throw new Exception("Markets exist");
            }

            await _mediator.Send(new MakeIndexerLockCommand());

            await _mediator.Send(new ProcessOdxDeploymentTransactionCommand(request.OdxDeploymentTxHash));
            await _mediator.Send(new ProcessDeployerDeploymentTransactionCommand(request.MarketDeployerDeploymentTxHash));
            await _mediator.Send(new ProcessLatestBlocksCommand(_network));

            await _mediator.Send(new MakeIndexerUnlockCommand());

            return NoContent();
        }
    }
}
