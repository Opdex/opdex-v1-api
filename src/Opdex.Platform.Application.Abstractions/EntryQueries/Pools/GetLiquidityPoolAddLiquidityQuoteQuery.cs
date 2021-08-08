using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetLiquidityPoolAddLiquidityQuoteQuery : IRequest<string>
    {
        public GetLiquidityPoolAddLiquidityQuoteQuery(string amountIn, string tokenIn, string pool, string market)
        {
            if (!amountIn.IsNumeric())
            {
                throw new ArgumentException("Amount in must only contain numeric digits.", nameof(amountIn));
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
