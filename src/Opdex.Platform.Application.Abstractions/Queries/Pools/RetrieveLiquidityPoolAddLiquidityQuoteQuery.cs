using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveLiquidityPoolAddLiquidityQuoteQuery : IRequest<string>
    {
        public RetrieveLiquidityPoolAddLiquidityQuoteQuery(string amountIn, string tokenIn, string pool, string market)
        {
            if (!amountIn.HasValue())
            {
                throw new ArgumentException(nameof(amountIn));
            }
            
            if (!tokenIn.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }
            
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }
            
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            AmountIn = amountIn;
            TokenIn = tokenIn;
            Pool = pool;
            Market = market;
        }
        
        public string AmountIn { get; }
        public string TokenIn { get; }
        public string Pool { get; }
        public string Market { get; }
    }
}