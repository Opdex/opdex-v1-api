using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools
{
    public class GetLiquidityPoolAddLiquidityQuoteQueryHandler : IRequestHandler<GetLiquidityPoolAddLiquidityQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolAddLiquidityQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(GetLiquidityPoolAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            var tokenInIsCrs = request.TokenIn.EqualsIgnoreCase(TokenConstants.Cirrus.Address);
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id), cancellationToken);
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Pool), cancellationToken);

            var tokenOut = tokenInIsCrs
                ? await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId), cancellationToken)
                : await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address), cancellationToken);

            var amountIn = request.AmountIn.ToSatoshis(tokenIn.Decimals);

            var quote = await _mediator.Send(new RetrieveLiquidityPoolAddLiquidityQuoteQuery(amountIn, request.TokenIn,
                                                                                             request.Pool, router.Address), cancellationToken);

            return quote.InsertDecimal(tokenOut.Decimals);
        }
    }
}
