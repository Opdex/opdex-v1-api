using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Summaries
{
    public class SelectLiquidityPoolSummaryByLiquidityPoolIdQuery : FindQuery<LiquidityPoolSummary>
    {
        public SelectLiquidityPoolSummaryByLiquidityPoolIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentNullException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public long LiquidityPoolId { get; }
    }
}
