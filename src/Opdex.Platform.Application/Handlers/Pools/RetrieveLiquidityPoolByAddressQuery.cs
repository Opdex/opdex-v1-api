using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
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
            var token = await _mediator.Send(new SelectLiquidityPoolByAddressQuery(request.Address), cancellationToken);

            return token;
        }
    }
}