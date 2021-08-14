using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolByLpTokenIdQuery : FindQuery<LiquidityPool>
    {
        public RetrieveLiquidityPoolByLpTokenIdQuery(long lpTokenId, bool findOrThrow = true) : base(findOrThrow)
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
