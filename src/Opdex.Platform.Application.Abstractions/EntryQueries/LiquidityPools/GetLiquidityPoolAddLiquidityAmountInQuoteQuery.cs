using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools
{
    public class GetLiquidityPoolAddLiquidityAmountInQuoteQuery : IRequest<FixedDecimal>
    {
        public GetLiquidityPoolAddLiquidityAmountInQuoteQuery(FixedDecimal amountIn, Address tokenIn, Address pool)
        {
            if (tokenIn == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (pool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            AmountIn = amountIn;
            TokenIn = tokenIn;
            Pool = pool;
        }

        public FixedDecimal AmountIn { get; }
        public Address TokenIn { get; }
        public Address Pool { get; }
    }
}
