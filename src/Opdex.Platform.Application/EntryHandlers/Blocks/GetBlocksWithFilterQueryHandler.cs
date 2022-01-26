using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class GetBlocksWithFilterQueryHandler : EntryFilterQueryHandler<GetBlocksWithFilterQuery, BlocksDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetBlocksWithFilterQueryHandler(ILogger<GetBlocksWithFilterQueryHandler> logger, IMapper mapper, IMediator mediator)
        : base(logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<BlocksDto> Handle(GetBlocksWithFilterQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _mediator.Send(new RetrieveBlocksWithFilterQuery(request.Cursor), cancellationToken);

        var blockResults = blocks.ToList();

        var cursor = BuildCursorDto(blockResults, request.Cursor, pointerSelector: result => result.Height);

        var dtos = _mapper.Map<IEnumerable<BlockDto>>(blockResults);

        return new BlocksDto { Blocks = dtos, Cursor = cursor };
    }
}
