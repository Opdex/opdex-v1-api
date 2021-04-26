using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class RetrieveActiveMarketSnapshotsByMarketIdQueryHandler : IRequestHandler<RetrieveActiveMarketSnapshotsByMarketIdQuery, IEnumerable<MarketSnapshot>>
    {
        private readonly IMediator _mediator;
        
        private RetrieveActiveMarketSnapshotsByMarketIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<IEnumerable<MarketSnapshot>> Handle(RetrieveActiveMarketSnapshotsByMarketIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectActiveMarketSnapshotsByMarketIdQuery(request.MarketId, request.Time), cancellationToken);
        }
    }
}