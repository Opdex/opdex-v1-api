using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets
{
    public class GetMarketPermissionsForAddressQueryHandler : IRequestHandler<GetMarketPermissionsForAddressQuery, IEnumerable<MarketPermissionType>>
    {
        private readonly IMediator _mediator;

        public GetMarketPermissionsForAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<MarketPermissionType>> Handle(GetMarketPermissionsForAddressQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            if (market.IsStakingMarket) throw new NotFoundException("Market address must represent a standard market.");

            var permissions = await _mediator.Send(new RetrieveMarketPermissionsByUserQuery(market.Id, request.Wallet), cancellationToken);
            return permissions;
        }
    }
}