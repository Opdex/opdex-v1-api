using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler : IRequestHandler<RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery, List<TokenSnapshot>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<List<TokenSnapshot>> Handle(RetrieveTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery request, CancellationToken cancellationToken)
        {
            var snapshotsQuery = new SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(request.TokenId, request.MarketId, request.Time);

            var snapshots = await _mediator.Send(snapshotsQuery, cancellationToken);

            return snapshots.ToList();
        }
    }
}