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
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Requests.Index;
using System.Linq;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using AutoMapper;
using Opdex.Platform.Common.Exceptions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly NetworkType _network;

        public IndexController(IMapper mapper, IMediator mediator, OpdexConfiguration opdexConfiguration)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        /// <summary>Get Latest Block</summary>
        /// <remarks>Retrieve the latest synced block.</remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Latest indexed block details.</response>
        /// <response code="404">No blocks have been indexed.</response>
        [HttpGet("latest-block")]
        [Authorize]
        [ProducesResponseType(typeof(BlockResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlockResponseModel>> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new GetLatestBlockQuery(), cancellationToken);

            return Ok(_mapper.Map<BlockResponseModel>(block));
        }

        /// <summary>Resync From Deployment</summary>
        /// <remarks>Processes the mined governance token and market deployer transactions then syncs to chain tip.</remarks>
        /// <remarks>
        /// For a successful redeployment, copy the mined token and market deployer deployment transaction hashes.
        /// Then clear all non-lookup tables of any data in the database. Leaving only `_type` tables populated.
        /// </remarks>
        /// <param name="request">The mined token and market deployer transaction hashes to look up.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Indexer resynced.</response>
        /// <response code="404">No markets exist to resync.</response>
        [HttpPost("resync-from-deployment")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResyncFromDeployment(ResyncFromDeploymentRequest request, CancellationToken cancellationToken)
        {
            // Todo: Spike and implement separate market vs wallet in JWT. Markets are currently required, they should not be.
            // If a new session starts, with no markets, a user cannot get authorized to hit this endpoint.
            // var admin = await _mediator.Send(new GetAdminByAddressQuery(_context.Wallet, findOrThrow: false), cancellationToken);
            // if (admin == null) return Unauthorized();

            var markets = await _mediator.Send(new RetrieveAllMarketsQuery(), cancellationToken);
            if (markets.Any())
            {
                throw new NotFoundException("No markets exist to resync.");
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
        /// <param name="request">Request to rewind back to specific block.</param>
        /// <response code="204">Indexer rewound.</response>
        [HttpPost("rewind")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Rewind(RewindRequest request)
        {
            await _mediator.Send(new MakeIndexerLockCommand());

            try
            {
                var rewound = await _mediator.Send(new CreateRewindToBlockCommand(request.Block));
                if (!rewound) throw new Exception("Indexer rewind unexpectedly failed.");
            }
            finally
            {
                await _mediator.Send(new MakeIndexerUnlockCommand());
            }

            return NoContent();
        }
    }
}
