using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools;

public class RetrieveLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, UInt256>
{
    private readonly IMediator _mediator;

    public RetrieveLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<UInt256> Handle(RetrieveLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
    {
        var isCrsIn = request.TokenIn.Address == Address.Cirrus;
        var isCrsOut = request.TokenOut.Address == Address.Cirrus;
        var isSrcToSrc = !isCrsIn && !isCrsOut;

        var market = await _mediator.Send(new SelectMarketByAddressQuery(request.Market, findOrThrow: true));

        // Get SrcSrc quote
        if (isSrcToSrc)
        {
            var tokenInPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.TokenIn.Id, market.Id), cancellationToken);
            var tokenOutPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.TokenOut.Id, market.Id), cancellationToken);
            var tokenInReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(tokenInPool.Address), cancellationToken);
            var tokenOutReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(tokenOutPool.Address), cancellationToken);

            return request.TokenInAmount > UInt256.Zero
                ? await _mediator.Send(new CallCirrusGetAmountOutMultiHopQuoteQuery(request.Router, request.TokenInAmount, tokenInReserves.Crs, tokenInReserves.Src, tokenOutReserves.Crs, tokenOutReserves.Src), cancellationToken)
                : await _mediator.Send(new CallCirrusGetAmountInMultiHopQuoteQuery(request.Router, request.TokenOutAmount, tokenOutReserves.Crs, tokenOutReserves.Src, tokenInReserves.Crs, tokenInReserves.Src), cancellationToken);
        }

        // Get CrsSrc quote
        var srcToken = isCrsOut ? request.TokenIn : request.TokenOut;
        var srcPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcToken.Id, market.Id), cancellationToken);
        var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(srcPool.Address), cancellationToken);

        var reservesIn = isCrsIn ? reserves.Crs : reserves.Src;
        var reservesOut = isCrsIn ? reserves.Src : reserves.Crs;

        return request.TokenInAmount > UInt256.Zero
            ? await _mediator.Send(new CallCirrusGetAmountOutStandardQuoteQuery(request.Router, request.TokenInAmount, reservesIn, reservesOut), cancellationToken)
            : await _mediator.Send(new CallCirrusGetAmountInStandardQuoteQuery(request.Router, request.TokenOutAmount, reservesIn, reservesOut), cancellationToken);
    }
}