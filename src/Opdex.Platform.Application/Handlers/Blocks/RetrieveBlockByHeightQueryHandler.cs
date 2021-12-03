using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks;

public class RetrieveBlockByHeightQueryHandler : IRequestHandler<RetrieveBlockByHeightQuery, Block>
{
    private readonly IMediator _mediator;

    public RetrieveBlockByHeightQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<Block> Handle(RetrieveBlockByHeightQuery request, CancellationToken cancellationToken)
    {
        var query = new SelectBlockByHeightQuery(request.Height, request.FindOrThrow);

        return _mediator.Send(query, cancellationToken);
    }
}