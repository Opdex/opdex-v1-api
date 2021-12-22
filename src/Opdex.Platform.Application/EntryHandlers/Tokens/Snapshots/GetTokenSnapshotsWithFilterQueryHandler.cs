using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;

public class GetTokenSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetTokenSnapshotsWithFilterQuery, TokenSnapshotsDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTokenSnapshotsWithFilterQueryHandler> _logger;

    public GetTokenSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper, ILogger<GetTokenSnapshotsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<TokenSnapshotsDto> Handle(GetTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: true), cancellationToken);

        var snapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(token.Id, default, request.Cursor), cancellationToken);

        var snapshotsResults = snapshots.ToList();

        _logger.LogTrace("Retrieved queried token snapshots");

        var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

        _logger.LogTrace("Returning {ResultCount} results", snapshotsResults.Count);

        var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<TokenSnapshotDto>(snapshot)).ToList();

        _logger.LogTrace("Assembled results");

        return new TokenSnapshotsDto { Snapshots = assembledResults, Cursor = cursorDto };
    }
}
