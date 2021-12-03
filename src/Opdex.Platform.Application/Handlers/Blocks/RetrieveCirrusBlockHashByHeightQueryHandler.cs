using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Application.Handlers.Blocks;

public class RetrieveCirrusBlockHashByHeightQueryHandler : IRequestHandler<RetrieveCirrusBlockHashByHeightQuery, Sha256>
{
    private readonly IMediator _mediator;

    public RetrieveCirrusBlockHashByHeightQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<Sha256> Handle(RetrieveCirrusBlockHashByHeightQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusGetBlockHashByHeightQuery(request.Height), cancellationToken);
    }
}