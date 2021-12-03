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

public class RetrieveSwapAmountInQueryHandler : IRequestHandler<RetrieveSwapAmountInQuery, UInt256>
{
    private readonly IMediator _mediator;

    public RetrieveSwapAmountInQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<UInt256> Handle(RetrieveSwapAmountInQuery request, CancellationToken cancellationToken)
    {
        return request.IsSingleHopQuery
            ? await GetAmountInSingleHop(request.Router, request.TokenIn, request.TokenOut, request.TokenOutAmount, cancellationToken)
            : await GetAmountInMultiHop(request.Router, request.TokenIn, request.TokenOut, request.TokenOutAmount, cancellationToken);
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