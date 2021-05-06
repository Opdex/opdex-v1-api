using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolAddLiquidityQuoteQueryHandler : IRequestHandler<GetLiquidityPoolAddLiquidityQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolAddLiquidityQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(GetLiquidityPoolAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
            
            var amountIn = request.AmountIn.ToSatoshis(tokenIn.Decimals);
            
            var quote = await _mediator.Send(new RetrieveLiquidityPoolAddLiquidityQuoteQuery(amountIn, request.TokenIn, request.Pool, request.Market), cancellationToken);

            // CRS in means SRC decimal based quote out
            var quoteDecimals = request.TokenIn == TokenConstants.Cirrus.Address ? tokenIn.Decimals : TokenConstants.Cirrus.Decimals;
            
            return quote.InsertDecimal(quoteDecimals);
        }
    }
}