using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Snapshots
{
    public class GetMarketSnapshotsWithFilterQueryHandler : IRequestHandler<GetMarketSnapshotsWithFilterQuery, IEnumerable<MarketSnapshotDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

        public GetMarketSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MarketSnapshotDto>> Handle(GetMarketSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress));

            var from = request.From ?? new DateTime(2021, 01, 01);
            var to = request.To ?? DateTime.UtcNow.ToEndOf(SnapshotType);

            var result = await _mediator.Send(new RetrieveMarketSnapshotsWithFilterQuery(market.Id,
                                                                                         from,
                                                                                         to,
                                                                                         SnapshotType), cancellationToken);

            return _mapper.Map<IEnumerable<MarketSnapshotDto>>(result.OrderBy(s => s.StartDate));
        }
    }
}
