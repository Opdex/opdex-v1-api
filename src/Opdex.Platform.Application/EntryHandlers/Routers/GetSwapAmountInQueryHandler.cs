using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Routers
{
    public class GetSwapAmountInQueryHandler : IRequestHandler<GetSwapAmountInQuery, FixedDecimal>
    {
        private readonly IMediator _mediator;

        public GetSwapAmountInQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<FixedDecimal> Handle(GetSwapAmountInQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn, findOrThrow: true), cancellationToken);
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut, findOrThrow: true), cancellationToken);

            var tokenOutAmount = request.TokenOutAmount.ToSatoshis(tokenOut.Decimals);

            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id, findOrThrow: true), cancellationToken);

            UInt256 amountIn = request.IsSingleHopQuery ? await GetAmountInSingleHop(router, tokenIn, tokenOut, tokenOutAmount, cancellationToken)
                                                        : await GetAmountInMultiHop(router, tokenIn, tokenOut, tokenOutAmount, cancellationToken);

            return amountIn.ToDecimal(tokenIn.Decimals);
        }

        private async Task<UInt256> GetAmountInSingleHop(MarketRouter router, Token tokenIn, Token tokenOut, UInt256 tokenOutAmount, CancellationToken cancellationToken)
        {
            var tokenInIsCrs = tokenIn.Address == Address.Cirrus;

            var srcTokenId = tokenInIsCrs ? tokenOut.Id : tokenIn.Id;
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, router.MarketId, findOrThrow: true), cancellationToken);

            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPool.Address), cancellationToken);
            var reservesTokenIn = tokenInIsCrs ? reserves.Crs : reserves.Src;
            var reservesTokenOut = tokenInIsCrs ? reserves.Src : reserves.Crs;

            return await _mediator.Send(new CallCirrusGetAmountInStandardQuoteQuery(router.Address, tokenOutAmount, reservesTokenIn, reservesTokenOut), cancellationToken);
        }

        private async Task<UInt256> GetAmountInMultiHop(MarketRouter router, Token tokenIn, Token tokenOut, UInt256 tokenOutAmount, CancellationToken cancellationToken)
        {
            var liquidityPoolIn = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenIn.Id, router.MarketId, findOrThrow: true), cancellationToken);
            var liquidityPoolOut = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenOut.Id, router.MarketId, findOrThrow: true), cancellationToken);

            var reservesPoolIn = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolIn.Address), cancellationToken);
            var reservesPoolOut = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolOut.Address), cancellationToken);

            return await _mediator.Send(new CallCirrusGetAmountInMultiHopQuoteQuery(router.Address, tokenOutAmount,
                                                                                    reservesPoolOut.Crs, reservesPoolOut.Src,
                                                                                    reservesPoolIn.Crs, reservesPoolIn.Src), cancellationToken);
        }
    }
}
