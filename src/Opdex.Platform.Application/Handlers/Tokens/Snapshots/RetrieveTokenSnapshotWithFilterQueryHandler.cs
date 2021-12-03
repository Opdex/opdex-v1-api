using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Application.Handlers.Tokens.Snapshots;

public class RetrieveTokenSnapshotWithFilterQueryHandler : IRequestHandler<RetrieveTokenSnapshotWithFilterQuery, TokenSnapshot>
{
    private readonly IMediator _mediator;

    public RetrieveTokenSnapshotWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<TokenSnapshot> Handle(RetrieveTokenSnapshotWithFilterQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectTokenSnapshotWithFilterQuery(request.TokenId, request.MarketId, request.DateTime, request.SnapshotType), cancellationToken);
    }
}