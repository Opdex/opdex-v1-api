using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveLiquidityPoolAddLiquidityQuoteQuery : IRequest<string>
    {
        public RetrieveLiquidityPoolAddLiquidityQuoteQuery(string amountIn, string tokenIn, string pool, string router)
        {
            if (!amountIn.HasValue())
            {
                throw new ArgumentNullException(nameof(amountIn));
            }

            if (!tokenIn.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenIn));
            }

            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (!router.HasValue())
            {
                throw new ArgumentNullException(nameof(router));
            }

            AmountIn = amountIn;
            TokenIn = tokenIn;
            Pool = pool;
            Router = router;
        }

        public string AmountIn { get; }
        public string TokenIn { get; }
        public string Pool { get; }
        public string Router { get; }
    }
}
