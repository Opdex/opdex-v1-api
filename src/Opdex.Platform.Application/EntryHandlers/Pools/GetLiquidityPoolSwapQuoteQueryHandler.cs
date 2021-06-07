using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<GetLiquidityPoolSwapQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(GetLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn, findOrThrow: true), cancellationToken);
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut, findOrThrow: true), cancellationToken);

            var tokenInAmount = request.TokenInAmount.HasValue() ? request.TokenInAmount.ToSatoshis(tokenIn.Decimals) : null;
            var tokenOutAmount = request.TokenOutAmount.HasValue() ? request.TokenOutAmount.ToSatoshis(tokenOut.Decimals) : null;

            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id, findOrThrow: true), cancellationToken);
            
            var query = new RetrieveLiquidityPoolSwapQuoteQuery(tokenIn, tokenOut, tokenInAmount, tokenOutAmount, request.Market, router.Address);

            var quote = await _mediator.Send(query, cancellationToken);

            return tokenInAmount.HasValue() ? quote.InsertDecimal(tokenOut.Decimals) : quote.InsertDecimal(tokenIn.Decimals);
        }
    }
}