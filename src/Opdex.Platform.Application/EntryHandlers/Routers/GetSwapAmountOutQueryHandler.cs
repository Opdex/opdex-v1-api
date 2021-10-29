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
    public class GetSwapAmountOutQueryHandler : IRequestHandler<GetSwapAmountOutQuery, FixedDecimal>
    {
        private readonly IMediator _mediator;

        public GetSwapAmountOutQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<FixedDecimal> Handle(GetSwapAmountOutQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn, findOrThrow: true), cancellationToken);
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut, findOrThrow: true), cancellationToken);

            var tokenInAmount = request.TokenInAmount.ToSatoshis(tokenOut.Decimals);

            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id, findOrThrow: true), cancellationToken);

            UInt256 amountOut = request.IsSingleHopQuery ? await GetAmountOutSingleHop(router, tokenIn, tokenInAmount, tokenOut, cancellationToken)
                                                         : await GetAmountOutMultiHop(router, tokenIn, tokenInAmount, tokenOut, cancellationToken);

            return amountOut.ToDecimal(tokenIn.Decimals);
        }

        private async Task<UInt256> GetAmountOutSingleHop(MarketRouter router, Token tokenIn, UInt256 tokenInAmount, Token tokenOut, CancellationToken cancellationToken)
        {
            var tokenInIsCrs = tokenIn.Address == Address.Cirrus;

            var srcTokenId = tokenInIsCrs ? tokenOut.Id : tokenIn.Id;
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, router.MarketId, findOrThrow: true), cancellationToken);

            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPool.Address));
            var reservesTokenIn = tokenInIsCrs ? reserves.Crs : reserves.Src;
            var reservesTokenOut = tokenInIsCrs ? reserves.Src : reserves.Crs;

            return await _mediator.Send(new CallCirrusGetAmountOutStandardQuoteQuery(router.Address, tokenInAmount, reservesTokenIn, reservesTokenOut), cancellationToken);
        }

        private async Task<UInt256> GetAmountOutMultiHop(MarketRouter router, Token tokenIn, UInt256 tokenInAmount, Token tokenOut, CancellationToken cancellationToken)
        {
            var liquidityPoolIn = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenIn.Id, router.MarketId, findOrThrow: true), cancellationToken);
            var liquidityPoolOut = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenOut.Id, router.MarketId, findOrThrow: true), cancellationToken);

            var reservesPoolIn = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolIn.Address));
            var reservesPoolOut = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolOut.Address));

            return await _mediator.Send(new CallCirrusGetAmountOutMultiHopQuoteQuery(router.Address, tokenInAmount,
                                                                                     reservesPoolIn.Crs, reservesPoolIn.Src,
                                                                                     reservesPoolOut.Crs, reservesPoolOut.Src));
        }
    }
}
