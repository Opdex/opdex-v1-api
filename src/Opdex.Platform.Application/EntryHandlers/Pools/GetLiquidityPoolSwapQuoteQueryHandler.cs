using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<GetLiquidityPoolSwapQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<string> Handle(GetLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveLiquidityPoolSwapQuoteQuery(request.TokenIn, request.TokenOut, request.TokenInAmount, request.TokenOutAmount, request.TokenInPool, request.TokenOutPool, request.Market);

            return _mediator.Send(query, cancellationToken);
        }
    }
}