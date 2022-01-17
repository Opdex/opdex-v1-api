using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools;

public class GetLiquidityAmountInQuoteQueryHandler : IRequestHandler<GetLiquidityAmountInQuoteQuery, FixedDecimal>
{
    private readonly IMediator _mediator;

    public GetLiquidityAmountInQuoteQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<FixedDecimal> Handle(GetLiquidityAmountInQuoteQuery request, CancellationToken cancellationToken)
    {
        var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Pool), cancellationToken);
        var summary = await _mediator.Send(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(pool.Id), cancellationToken);
        if (summary.LockedCrs == UInt256.Zero)
        {
            throw new InvalidDataException("pool", "Pool has no liquidity, any token amount can be added.");
        }

        var tokenInIsCrs = request.TokenIn == Address.Cirrus;
        var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
        var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId), cancellationToken);
        var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id), cancellationToken);

        var tokenOut = tokenInIsCrs
            ? await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId), cancellationToken)
            : await _mediator.Send(new RetrieveTokenByAddressQuery(Address.Cirrus), cancellationToken);

        var amountIn = request.AmountIn.ToSatoshis(tokenIn.Decimals);

        var quote = await _mediator.Send(new RetrieveLiquidityAmountInQuoteQuery(amountIn, request.TokenIn,
                                                                                 request.Pool, router.Address), cancellationToken);

        return quote.ToDecimal(tokenOut.Decimals);
    }
}
