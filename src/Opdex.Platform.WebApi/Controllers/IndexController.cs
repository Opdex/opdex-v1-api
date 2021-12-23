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
using AutoMapper;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.WebApi.Models.Responses.Index;

namespace Opdex.Platform.WebApi.Controllers;

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

    /// <summary>Get Indexer Status</summary>
    /// <remarks>Retrieves status of the indexer.</remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Indexer status details.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IndexerStatusResponseModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<IndexerStatusResponseModel>> GetIndexerStatus(CancellationToken cancellationToken)
    {
        var status = await _mediator.Send(new GetIndexerStatusQuery(), cancellationToken);

        return Ok(_mapper.Map<IndexerStatusResponseModel>(status));
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
    /// <response code="400">Markets already indexed.</response>
    /// <response code="403">You don't have permission to carry out this request.</response>
    [HttpPost("resync-from-deployment")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ResyncFromDeployment(ResyncFromDeploymentRequest request, CancellationToken cancellationToken)
    {
        var markets = await _mediator.Send(new RetrieveAllMarketsQuery(), cancellationToken);
        if (markets.Any())
        {
            throw new AlreadyIndexedException("Markets already indexed.");
        }

        var bestBlock = await _mediator.Send(new GetBestBlockReceiptQuery(), CancellationToken.None);

        var locked = await _mediator.Send(new MakeIndexerLockCommand(IndexLockReason.Deploying), CancellationToken.None);
        if (!locked) throw new IndexingAlreadyRunningException();

        try
        {
            await _mediator.Send(new ProcessGovernanceDeploymentTransactionCommand(request.MinedTokenDeploymentHash), CancellationToken.None);
            await _mediator.Send(new ProcessCoreDeploymentTransactionCommand(request.MarketDeployerDeploymentTxHash), CancellationToken.None);
            await _mediator.Send(new ProcessLatestBlocksCommand(bestBlock, _network), CancellationToken.None);
        }
        finally
        {
            await _mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> Rewind(RewindRequest request)
    {
        var tryLock = await _mediator.Send(new MakeIndexerLockCommand(IndexLockReason.Searching), CancellationToken.None);
        if (!tryLock) throw new IndexingAlreadyRunningException();

        try
        {
            var bestBlock = await _mediator.Send(new GetBestBlockReceiptQuery(), CancellationToken.None);

            // Get our latest synced block from the database
            var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: true), CancellationToken.None);
            var currentIndexedHeight = currentBlock.Height;

            // Walk backward through our database blocks until we find one that can be found at the FN
            ulong reorgLength = 0;
            while (bestBlock is null && ++reorgLength < IndexerBackgroundService.MaxReorg)
            {
                currentBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(currentIndexedHeight - reorgLength), CancellationToken.None);
                bestBlock = await _mediator.Send(new RetrieveCirrusBlockReceiptByHashQuery(currentBlock.Hash, findOrThrow: false), CancellationToken.None);
            }

            if (bestBlock is null) throw new MaximumReorgException();

            // Rewind our data back to the latest matching block
            await _mediator.Send(new MakeIndexerLockReasonCommand(IndexLockReason.Rewinding), CancellationToken.None);
            var rewound = await _mediator.Send(new CreateRewindToBlockCommand(bestBlock.Height), CancellationToken.None);
            if (!rewound) throw new Exception($"Failure rewinding database to block height: {bestBlock.Height}");

            // Begin resyncing
            await _mediator.Send(new MakeIndexerLockReasonCommand(IndexLockReason.Resyncing), CancellationToken.None);
            await _mediator.Send(new ProcessLatestBlocksCommand(bestBlock, _network), CancellationToken.None);
        }
        finally
        {
            await _mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
        }

        return NoContent();
    }
}
