using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Application.Handlers.Tokens.Snapshots
{
    public class RetrieveTokenSnapshotsWithFilterQueryHandler : IRequestHandler<RetrieveTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>
    {
        private readonly IMediator _mediator;

        public RetrieveTokenSnapshotsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<TokenSnapshot>> Handle(RetrieveTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectTokenSnapshotsWithFilterQuery(request.TokenId, request.MarketId, request.Cursor), cancellationToken);
        }
    }
}
