using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolAddLiquidityAmountInQuoteQuery : IRequest<UInt256>
    {
        public RetrieveLiquidityPoolAddLiquidityAmountInQuoteQuery(UInt256 amountIn, Address tokenIn, Address pool, Address router)
        {
            if (tokenIn == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (pool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router));
            }

            AmountIn = amountIn;
            TokenIn = tokenIn;
            Pool = pool;
            Router = router;
        }

        public UInt256 AmountIn { get; }
        public Address TokenIn { get; }
        public Address Pool { get; }
        public Address Router { get; }
    }
}
