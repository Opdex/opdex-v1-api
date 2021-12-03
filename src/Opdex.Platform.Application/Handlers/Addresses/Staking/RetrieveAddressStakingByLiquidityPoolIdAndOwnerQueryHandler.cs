using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Staking;

public class RetrieveAddressStakingByLiquidityPoolIdAndOwnerQueryHandler
    : IRequestHandler<RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery, AddressStaking>
{
    private readonly IMediator _mediator;

    public RetrieveAddressStakingByLiquidityPoolIdAndOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<AddressStaking> Handle(RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
    {
        var query = new SelectAddressStakingByLiquidityPoolIdAndOwnerQuery(request.LiquidityPoolId, request.Owner, request.FindOrThrow);

        return _mediator.Send(query, cancellationToken);
    }
}