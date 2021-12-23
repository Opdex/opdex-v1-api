using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Index;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Indexer;

public class GetIndexerStatusQueryHandler : IRequestHandler<GetIndexerStatusQuery, IndexerStatusDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetIndexerStatusQueryHandler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IndexerStatusDto> Handle(GetIndexerStatusQuery request, CancellationToken cancellationToken)
    {
        var block = await _mediator.Send(new RetrieveLatestBlockQuery(false), cancellationToken);
        var indexLock = await _mediator.Send(new RetrieveIndexerLockQuery(), cancellationToken);

        var blockDto = _mapper.Map<BlockDto>(block);
        var lockStatusDto = _mapper.Map<IndexerStatusDto>(indexLock);
        lockStatusDto.LatestBlock = blockDto;
        return lockStatusDto;
    }
}
