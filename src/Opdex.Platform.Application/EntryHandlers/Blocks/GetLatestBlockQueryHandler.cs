using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks;

public class GetLatestBlockQueryHandler : IRequestHandler<GetLatestBlockQuery, BlockDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetLatestBlockQueryHandler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    public async Task<BlockDto> Handle(GetLatestBlockQuery request, CancellationToken cancellationToken)
    {
        var block = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: false), cancellationToken);
        if (block is null) throw new NotFoundException("No blocks have been indexed.");
        return _mapper.Map<BlockDto>(block);
    }
}