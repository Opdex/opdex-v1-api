using AutoMapper;
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
    public class RetrieveMarketSnapshotsWithFilterQueryHandler: IRequestHandler<RetrieveMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshot>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RetrieveMarketSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<IEnumerable<MarketSnapshot>> Handle(RetrieveMarketSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMarketSnapshotsWithFilterQuery(request.MarketId, request.StartDate, request.EndDate, request.SnapshotType), cancellationToken);
        }
    }
}
