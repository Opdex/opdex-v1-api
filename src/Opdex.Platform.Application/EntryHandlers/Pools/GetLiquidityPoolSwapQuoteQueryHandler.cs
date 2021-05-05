using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolSwapQuoteQueryHandler : IRequestHandler<GetLiquidityPoolSwapQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolSwapQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(GetLiquidityPoolSwapQuoteQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut), cancellationToken);

            var tokenInAmount = request.TokenInAmount.HasValue() ? request.TokenInAmount.ToSatoshis(tokenIn.Decimals) : null;
            var tokenOutAmount = request.TokenOutAmount.HasValue() ? request.TokenOutAmount.ToSatoshis(tokenOut.Decimals) : null;
            
            var query = new RetrieveLiquidityPoolSwapQuoteQuery(tokenIn, tokenOut, tokenInAmount, tokenOutAmount, request.Market);

            var quote = await _mediator.Send(query, cancellationToken);

            return tokenInAmount.HasValue() ? quote.InsertDecimal(tokenOut.Decimals) : quote.InsertDecimal(tokenIn.Decimals);
        }
    }
}