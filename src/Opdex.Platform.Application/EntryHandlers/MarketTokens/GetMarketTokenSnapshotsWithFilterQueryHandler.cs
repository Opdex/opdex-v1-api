using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MarketTokens;

public class GetMarketTokenSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetMarketTokenSnapshotsWithFilterQuery, MarketTokenSnapshotsDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetMarketTokenSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper, ILogger<GetMarketTokenSnapshotsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override async Task<MarketTokenSnapshotsDto> Handle(GetMarketTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
    {
        if (request.Token == Address.Cirrus) throw new InvalidDataException("token", "Market snapshot history is not collected for the base token.");

        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: true), cancellationToken);
        var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);

        var snapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(token.Id, market.Id, request.Cursor), cancellationToken);

        var snapshotsResults = snapshots.ToList();

        var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

        var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<TokenSnapshotDto>(snapshot)).ToList();

        return new MarketTokenSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
    }
}
