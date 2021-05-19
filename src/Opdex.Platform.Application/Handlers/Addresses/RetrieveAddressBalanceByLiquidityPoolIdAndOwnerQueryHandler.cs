using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler
        : IRequestHandler<RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQuery, AddressBalance>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<AddressBalance> Handle(RetrieveAddressBalanceByLiquidityPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAddressBalanceByLiquidityPoolIdAndOwnerQuery(request.LiquidityPoolId, request.Owner, request.FindOrThrow);
            
            return _mediator.Send(query, cancellationToken);
        }
    }
}