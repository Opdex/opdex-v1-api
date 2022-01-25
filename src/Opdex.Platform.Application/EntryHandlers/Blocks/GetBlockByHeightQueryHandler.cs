using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class GetBlockByHeightQueryHandler : IRequestHandler<GetBlockByHeightQuery, BlockDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetBlockByHeightQueryHandler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<BlockDto> Handle(GetBlockByHeightQuery request, CancellationToken cancellationToken)
    {
        var block = await _mediator.Send(new RetrieveBlockByHeightQuery(request.Height, true), cancellationToken);
        return _mapper.Map<BlockDto>(block);
    }
}
