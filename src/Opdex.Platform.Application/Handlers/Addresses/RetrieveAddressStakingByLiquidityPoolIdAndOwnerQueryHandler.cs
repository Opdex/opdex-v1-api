using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
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
            return _mediator.Send(new SelectAddressStakingByLiquidityPoolIdAndOwnerQuery(request.LiquidityPoolId, request.Owner), cancellationToken);
        }
    }
}