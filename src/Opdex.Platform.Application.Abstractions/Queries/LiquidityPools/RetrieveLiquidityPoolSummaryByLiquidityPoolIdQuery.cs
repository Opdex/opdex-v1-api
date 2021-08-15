using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery : FindQuery<LiquidityPoolSummary>
    {
        public RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public long LiquidityPoolId { get; }
    }
}
