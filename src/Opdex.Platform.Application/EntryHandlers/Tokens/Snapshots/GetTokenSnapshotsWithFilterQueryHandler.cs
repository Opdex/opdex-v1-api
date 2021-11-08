using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class GetTokenSnapshotsWithFilterQueryHandler : EntryFilterQueryHandler<GetTokenSnapshotsWithFilterQuery, TokenSnapshotsDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetTokenSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<TokenSnapshotsDto> Handle(GetTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: true), cancellationToken);

            var snapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(token.Id, default, request.Cursor), cancellationToken);

            var snapshotsResults = snapshots.ToList();

            var cursorDto = BuildCursorDto(snapshotsResults, request.Cursor, pointerSelector: result => (result.StartDate, result.Id));

            var assembledResults = snapshotsResults.Select(snapshot => _mapper.Map<TokenSnapshotDto>(snapshot)).ToList();

            return new TokenSnapshotsDto { Token = token.Address, Snapshots = assembledResults, Cursor = cursorDto };
        }
    }
}
