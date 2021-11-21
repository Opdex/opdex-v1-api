using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Snapshots
{
    public class GetMarketSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetMarketSnapshotsWithFilterQuery, MarketSnapshotsDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetMarketSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<MarketSnapshotsDto> Handle(GetMarketSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);

            var snapshots = await _mediator.Send(new RetrieveMarketSnapshotsWithFilterQuery(market.Id, request.Cursor), cancellationToken);

            var snapshotsResults = snapshots.ToList();

            var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

            var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<MarketSnapshotDto>(snapshot)).ToList();

            return new MarketSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
        }
    }
}
