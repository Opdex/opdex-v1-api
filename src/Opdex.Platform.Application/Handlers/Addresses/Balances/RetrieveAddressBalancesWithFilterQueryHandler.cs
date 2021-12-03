using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Balances;

public class RetrieveAddressBalancesWithFilterQueryHandler : IRequestHandler<RetrieveAddressBalancesWithFilterQuery, IEnumerable<AddressBalance>>
{
    private readonly IMediator _mediator;

    public RetrieveAddressBalancesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<AddressBalance>> Handle(RetrieveAddressBalancesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectAddressBalancesWithFilterQuery(request.Address, request.Cursor), cancellationToken);
    }
}