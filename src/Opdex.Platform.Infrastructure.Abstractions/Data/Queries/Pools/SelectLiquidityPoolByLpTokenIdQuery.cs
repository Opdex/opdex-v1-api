using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectLiquidityPoolByLpTokenIdQuery : FindQuery<LiquidityPool>
    {
        public SelectLiquidityPoolByLpTokenIdQuery(long lpTokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (lpTokenId < 1)
            {
                throw new ArgumentNullException(nameof(lpTokenId), $"{nameof(lpTokenId)} cannot be null.");
            }

            LpTokenId = lpTokenId;
        }

        public long LpTokenId { get; }
    }
}
