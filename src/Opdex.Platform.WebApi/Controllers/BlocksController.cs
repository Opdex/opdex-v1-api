using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.WebApi.Caching;
using Opdex.Platform.WebApi.Models.Requests.Blocks;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/blocks")]
[ApiVersion("1")]
public class BlocksController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public BlocksController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>Get Blocks</summary>
    /// <remarks>Retrieves blocks that are indexed</remarks>
    /// <param name="filters">Filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Block results</returns>
    [HttpGet]
    [CacheUntilNextBlock(CacheType.Public)]
    public async Task<ActionResult<BlocksResponseModel>> GetBlocks(
        [FromQuery] BlockFilterParameters filters,
        CancellationToken cancellationToken)
    {
        var blocks = await _mediator.Send(new GetBlocksWithFilterQuery(filters.BuildCursor()), cancellationToken);
        var results = _mapper.Map<BlocksResponseModel>(blocks);
        return Ok(results);
    }

    /// <summary>Get Block</summary>
    /// <remarks>Retrieves a block that is indexed by its height</remarks>
    /// <param name="height">Height of the block</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Block details</returns>
    [HttpGet("{height}")]
    [CacheUntilNextBlock(CacheType.Public)]
    public async Task<ActionResult<BlockResponseModel>> GetBlock(ulong height, CancellationToken cancellationToken)
    {
        if (height == 0) throw new InvalidDataException(nameof(height), "Height must be greater than 0.");
        var block = await _mediator.Send(new GetBlockByHeightQuery(height), cancellationToken);
        var result = _mapper.Map<BlockResponseModel>(block);
        return Ok(result);
    }
}
