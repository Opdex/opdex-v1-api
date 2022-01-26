using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Blocks;

public class RetrieveBlocksWithFilterQueryHandler : IRequestHandler<RetrieveBlocksWithFilterQuery, IEnumerable<Block>>
{
    private readonly IMediator _mediator;

    public RetrieveBlocksWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<Block>> Handle(RetrieveBlocksWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectBlocksWithFilterQuery(request.Cursor), cancellationToken);
    }
}
