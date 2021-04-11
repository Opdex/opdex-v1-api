using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Pools;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Core.Application.Handlers.Pools
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