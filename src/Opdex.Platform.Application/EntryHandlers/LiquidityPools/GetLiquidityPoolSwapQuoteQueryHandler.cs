using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools
{
    public class GetLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<GetLiquidityPoolSwapQuoteQuery, FixedDecimal>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<FixedDecimal> Handle(GetLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn, findOrThrow: true), cancellationToken);
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut, findOrThrow: true), cancellationToken);

            var tokenInAmount = request.TokenInAmount.ToSatoshis(tokenIn.Decimals);
            var tokenOutAmount = request.TokenOutAmount.ToSatoshis(tokenOut.Decimals);

            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id, findOrThrow: true), cancellationToken);

            var query = new RetrieveLiquidityPoolSwapQuoteQuery(tokenIn, tokenOut, tokenInAmount, tokenOutAmount, request.Market, router.Address);

            var quote = await _mediator.Send(query, cancellationToken);

            return tokenInAmount > UInt256.Zero ? quote.ToDecimal(tokenOut.Decimals) : quote.ToDecimal(tokenIn.Decimals);
        }
    }
}
