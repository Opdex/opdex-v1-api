using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Snapshots;

public class GetMarketSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetMarketSnapshotsWithFilterQuery, MarketSnapshotsDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMarketSnapshotsWithFilterQueryHandler> _logger;

    public GetMarketSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper, ILogger<GetMarketSnapshotsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<MarketSnapshotsDto> Handle(GetMarketSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);

        var snapshots = await _mediator.Send(new RetrieveMarketSnapshotsWithFilterQuery(market.Id, request.Cursor), cancellationToken);

        var snapshotsResults = snapshots.ToList();

        _logger.LogTrace("Retrieved queried market snapshots");

        var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

        _logger.LogTrace("Returning {ResultCount} results", snapshotsResults.Count);

        var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<MarketSnapshotDto>(snapshot)).ToList();

        _logger.LogTrace("Assembled results");

        return new MarketSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
    }
}
