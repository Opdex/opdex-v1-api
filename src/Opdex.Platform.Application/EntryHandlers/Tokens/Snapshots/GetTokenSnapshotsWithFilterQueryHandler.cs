using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots
{
    public class GetTokenSnapshotsWithFilterQueryHandler
        : IRequestHandler<GetTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshotDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetTokenSnapshotsWithFilterQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenSnapshotDto>> Handle(GetTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var start = now.StartDateOfDuration(request.TimeSpan).ToStartOf(request.SnapshotType);
            var end = DateTime.UtcNow.ToEndOf(request.SnapshotType);

            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenAddress.ToString()), cancellationToken);

            var market = token.Address == TokenConstants.Cirrus.Address
                    ? null
                    : await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress), cancellationToken);


            var snapshots = await _mediator.Send(new RetrieveTokenSnapshotsWithFilterQuery(token.Id,
                                                                                           market?.Id ?? 0,
                                                                                           start,
                                                                                           end,
                                                                                           request.SnapshotType), cancellationToken);

            return _mapper.Map<IEnumerable<TokenSnapshotDto>>(snapshots.OrderBy(p => p.StartDate));
        }
    }
}
