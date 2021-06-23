using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Snapshots
{
    public class RetrieveMarketSnapshotWithFilterQueryHandler : IRequestHandler<RetrieveMarketSnapshotWithFilterQuery, MarketSnapshot>
    {
        private readonly IMediator _mediator;

        public RetrieveMarketSnapshotWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<MarketSnapshot> Handle(RetrieveMarketSnapshotWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketSnapshotWithFilterQuery(request.MarketId, request.DateTime, request.SnapshotType), cancellationToken);
        }
    }
}
