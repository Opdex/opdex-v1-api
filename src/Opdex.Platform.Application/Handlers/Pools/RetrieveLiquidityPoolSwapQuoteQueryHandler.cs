using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<string> Handle(RetrieveLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            const string crs = "CRS";
            var isCrsIn = request.TokenIn == crs;
            var isCrsOut = request.TokenOut == crs;
            var isSrcToSrc = !isCrsIn && !isCrsOut;
            var result = "0";

            if (isSrcToSrc)
            {
                // Get reserves from both pools
                // Get srcsrc quote
            }
            else
            {
                // Get reserves for the non CRS pool
                // Get CrsSrc quote
            }

            return Task.FromResult(result);
        }
    }
}