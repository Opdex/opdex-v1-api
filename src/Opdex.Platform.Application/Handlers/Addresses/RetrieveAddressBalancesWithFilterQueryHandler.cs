using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressBalancesWithFilterQueryHandler : IRequestHandler<RetrieveAddressBalancesWithFilterQuery, List<AddressBalance>>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressBalancesWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<List<AddressBalance>> Handle(RetrieveAddressBalancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAddressBalancesWithFilterQuery(request.Wallet, request.Tokens, request.IncludeLpTokens, request.IncludeZeroBalances,
                                                                           request.Direction, request.Limit, request.Next, request.Previous), cancellationToken);
        }
    }
}
