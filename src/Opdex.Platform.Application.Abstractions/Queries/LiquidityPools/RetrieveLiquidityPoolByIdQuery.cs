using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolByIdQuery : FindQuery<LiquidityPool>
    {
        public RetrieveLiquidityPoolByIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentNullException(nameof(liquidityPoolId));
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public long LiquidityPoolId { get; }
    }
}
