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
using Opdex.Platform.WebApi.Models.Responses.Indexer;
using System.Linq;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly NetworkType _network;
        private readonly string _instanceId;

        public IndexController(IMediator mediator, OpdexConfiguration opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
            _instanceId = opdexConfiguration?.InstanceId ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        /// <summary>Get Latest Block</summary>
        /// <remarks>Retrieve the latest synced block.</remarks>
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

        /// <summary>Get Instance Identity</summary>
        /// <remarks>Retrieve the identifier of this specific host instance.</remarks>
        /// <returns>GUID as string identifier</returns>
        [HttpGet("instance-identity")]
        [Authorize]
        [ProducesResponseType(typeof(InstanceIdentityResponseModel), StatusCodes.Status200OK)]
        public ActionResult<InstanceIdentityResponseModel> InstanceIdentity()
        {
            return Ok(new InstanceIdentityResponseModel { Identity = _instanceId });
        }

        /// <summary>Resync From Deployment</summary>
        /// <remarks>Processes the mined governance token and market deployer transactions then syncs to chain tip.</remarks>
        /// <remarks>
        /// For a successful redeployment, copy the mined token and market deployer deployment transaction hashes.
        /// Then clear all non-lookup tables of any data in the database. Leaving only `_type` tables populated.
        /// </remarks>
        /// <param name="request">The mined token and market deployer transaction hashes to look up.</param>
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

            try
            {
                await _mediator.Send(new ProcessGovernanceDeploymentTransactionCommand(request.MinedTokenDeploymentHash));
                await _mediator.Send(new ProcessCoreDeploymentTransactionCommand(request.MarketDeployerDeploymentTxHash));
                await _mediator.Send(new ProcessLatestBlocksCommand(_network));
            }
            finally
            {
                await _mediator.Send(new MakeIndexerUnlockCommand());
            }

            return NoContent();
        }

        /// <summary>Rewind to Block</summary>
        /// <param name="request">Request object containing the block height ot rewind too.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        [HttpPost("rewind")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Rewind(RewindRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new MakeIndexerLockCommand());

            try
            {
                await _mediator.Send(new CreateRewindToBlockCommand(request.Block));
            }
            finally
            {
                await _mediator.Send(new MakeIndexerUnlockCommand());
            }

            return NoContent();
        }
    }
}
