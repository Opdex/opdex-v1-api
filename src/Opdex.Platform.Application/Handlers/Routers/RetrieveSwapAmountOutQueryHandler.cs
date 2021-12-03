using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Routers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Routers;

public class RetrieveSwapAmountOutQueryHandler : IRequestHandler<RetrieveSwapAmountOutQuery, UInt256>
{
    private readonly IMediator _mediator;

    public RetrieveSwapAmountOutQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<UInt256> Handle(RetrieveSwapAmountOutQuery request, CancellationToken cancellationToken)
    {
        return request.IsSingleHopQuery
            ? await GetAmountOutSingleHop(request.Router, request.TokenIn, request.TokenInAmount, request.TokenOut, cancellationToken)
            : await GetAmountOutMultiHop(request.Router, request.TokenIn, request.TokenInAmount, request.TokenOut, cancellationToken);
    }

    private async Task<UInt256> GetAmountOutSingleHop(MarketRouter router, Token tokenIn, UInt256 tokenInAmount, Token tokenOut, CancellationToken cancellationToken)
    {
        var tokenInIsCrs = tokenIn.Address == Address.Cirrus;

        var srcTokenId = tokenInIsCrs ? tokenOut.Id : tokenIn.Id;
        var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, router.MarketId, findOrThrow: true), cancellationToken);

        var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPool.Address), cancellationToken);
        var reservesTokenIn = tokenInIsCrs ? reserves.Crs : reserves.Src;
        var reservesTokenOut = tokenInIsCrs ? reserves.Src : reserves.Crs;

        return await _mediator.Send(new CallCirrusGetAmountOutStandardQuoteQuery(router.Address, tokenInAmount, reservesTokenIn, reservesTokenOut), cancellationToken);
    }

    private async Task<UInt256> GetAmountOutMultiHop(MarketRouter router, Token tokenIn, UInt256 tokenInAmount, Token tokenOut, CancellationToken cancellationToken)
    {
        var liquidityPoolIn = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenIn.Id, router.MarketId, findOrThrow: true), cancellationToken);
        var liquidityPoolOut = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(tokenOut.Id, router.MarketId, findOrThrow: true), cancellationToken);

        var reservesPoolIn = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolIn.Address), cancellationToken);
        var reservesPoolOut = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(liquidityPoolOut.Address), cancellationToken);

        return await _mediator.Send(new CallCirrusGetAmountOutMultiHopQuoteQuery(router.Address, tokenInAmount,
                                                                                 reservesPoolIn.Crs, reservesPoolIn.Src,
                                                                                 reservesPoolOut.Crs, reservesPoolOut.Src), cancellationToken);
    }
}