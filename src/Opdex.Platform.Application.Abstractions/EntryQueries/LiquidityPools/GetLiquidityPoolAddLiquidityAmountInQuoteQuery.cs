using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools
{
    public class GetLiquidityPoolAddLiquidityAmountInQuoteQuery : IRequest<FixedDecimal>
    {
        public GetLiquidityPoolAddLiquidityAmountInQuoteQuery(FixedDecimal amountIn, Address tokenIn, Address pool, Address market)
        {
            if (tokenIn == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (pool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market));
            }

            AmountIn = amountIn;
            TokenIn = tokenIn;
            Pool = pool;
            Market = market;
        }

        public FixedDecimal AmountIn { get; }
        public Address TokenIn { get; }
        public Address Pool { get; }
        public Address Market { get; }
    }
}
