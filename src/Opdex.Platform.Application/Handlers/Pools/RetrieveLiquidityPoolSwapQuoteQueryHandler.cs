using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolSwapQuoteQuery, string>
    {
        private const string crs = "CRS";
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(RetrieveLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var isCrsIn = request.TokenIn == crs;
            var isCrsOut = request.TokenOut == crs;
            var isSrcToSrc = !isCrsIn && !isCrsOut;

            // Get SrcSrc quote
            if (isSrcToSrc)
            {
                var tokenInReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(request.TokenInPool), cancellationToken);
                var tokenOutReserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(request.TokenOutPool), cancellationToken);

                return request.TokenInAmount.HasValue()
                    ? await _mediator.Send(new CallCirrusGetAmountOutMultiHopQuoteQuery(request.Market, request.TokenInAmount, tokenInReserves[0], tokenInReserves[1], tokenOutReserves[0], tokenOutReserves[1]), cancellationToken)
                    : await _mediator.Send(new CallCirrusGetAmountInMultiHopQuoteQuery(request.Market, request.TokenOutAmount, tokenOutReserves[0], tokenOutReserves[1], tokenInReserves[0], tokenInReserves[1]), cancellationToken);
            }
            
            // Get CrsSrc quote
            var pool = isCrsOut ? request.TokenInPool : request.TokenOutPool;
            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(pool), cancellationToken);

            var reservesIn = isCrsIn ? reserves[0] : reserves[1];
            var reservesOut = isCrsIn ? reserves[1] : reserves[0];
            
            return request.TokenInAmount.HasValue()
                ? await _mediator.Send(new CallCirrusGetAmountOutStandardQuoteQuery(request.Market, request.TokenInAmount, reservesIn, reservesOut), cancellationToken)
                : await _mediator.Send(new CallCirrusGetAmountInStandardQuoteQuery(request.Market, request.TokenOutAmount, reservesIn, reservesOut), cancellationToken);
        }
    }
}