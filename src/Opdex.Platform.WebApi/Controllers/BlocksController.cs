using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Common.Exceptions;
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

    [HttpGet]
    public async Task<ActionResult<BlocksResponseModel>> GetBlocks(
        [FromQuery] BlockFilterParameters filters,
        CancellationToken cancellationToken)
    {
        var blocks = await _mediator.Send(new GetBlocksWithFilterQuery(filters.BuildCursor()), cancellationToken);
        var results = _mapper.Map<BlocksResponseModel>(blocks);
        return Ok(results);
    }

    [HttpGet("{height}")]
    public async Task<ActionResult<BlockResponseModel>> GetBlock(ulong height, CancellationToken cancellationToken)
    {
        if (height == 0) throw new InvalidDataException(nameof(height), "Height must be greater than 0.");
        var block = await _mediator.Send(new GetBlockByHeightQuery(height), cancellationToken);
        var result = _mapper.Map<BlockResponseModel>(block);
        return Ok(result);
    }
}
