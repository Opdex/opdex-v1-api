using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolByAddressQueryHandler : IRequestHandler<RetrieveLiquidityPoolByAddressQuery, LiquidityPool>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<LiquidityPool> Handle(RetrieveLiquidityPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new SelectLiquidityPoolByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
        }
    }
}
