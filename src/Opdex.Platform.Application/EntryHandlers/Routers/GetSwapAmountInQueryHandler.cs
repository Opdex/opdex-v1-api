using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Routers;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Routers;

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

        UInt256 amountIn = await _mediator.Send(new RetrieveSwapAmountInQuery(router, tokenIn, tokenOut, tokenOutAmount), cancellationToken);

        return amountIn.ToDecimal(tokenIn.Decimals);
    }
}