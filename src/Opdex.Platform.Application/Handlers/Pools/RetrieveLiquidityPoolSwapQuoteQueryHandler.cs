using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(RetrieveLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var isCrsIn = request.TokenIn.Address == TokenConstants.Cirrus.Address;
            var isCrsOut = request.TokenOut.Address == TokenConstants.Cirrus.Address;
            var isSrcToSrc = !isCrsIn && !isCrsOut;

            var market = await _mediator.Send(new SelectMarketByAddressQuery(request.Market, findOrThrow: true));

            // Get SrcSrc quote
            if (isSrcToSrc)
            {
                var tokenInPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.TokenIn.Id, market.Id), cancellationToken);
                var tokenOutPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(request.TokenOut.Id, market.Id), cancellationToken);
                var tokenInReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(tokenInPool.Address), cancellationToken);
                var tokenOutReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(tokenOutPool.Address), cancellationToken);

                return request.TokenInAmount.HasValue()
                    ? await _mediator.Send(new CallCirrusGetAmountOutMultiHopQuoteQuery(request.Router, request.TokenInAmount, tokenInReserves[0], tokenInReserves[1], tokenOutReserves[0], tokenOutReserves[1]), cancellationToken)
                    : await _mediator.Send(new CallCirrusGetAmountInMultiHopQuoteQuery(request.Router, request.TokenOutAmount, tokenOutReserves[0], tokenOutReserves[1], tokenInReserves[0], tokenInReserves[1]), cancellationToken);
            }

            // Get CrsSrc quote
            var srcToken = isCrsOut ? request.TokenIn : request.TokenOut;
            var srcPool = await _mediator.Send(new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcToken.Id, market.Id), cancellationToken);
            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(srcPool.Address), cancellationToken);

            var reservesIn = isCrsIn ? reserves[0] : reserves[1];
            var reservesOut = isCrsIn ? reserves[1] : reserves[0];

            return request.TokenInAmount.HasValue()
                ? await _mediator.Send(new CallCirrusGetAmountOutStandardQuoteQuery(request.Router, request.TokenInAmount, reservesIn, reservesOut), cancellationToken)
                : await _mediator.Send(new CallCirrusGetAmountInStandardQuoteQuery(request.Router, request.TokenOutAmount, reservesIn, reservesOut), cancellationToken);
        }
    }
}
