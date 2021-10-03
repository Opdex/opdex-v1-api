using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools
{
    public class SelectMiningPoolByLiquidityPoolIdQuery : FindQuery<MiningPool>
    {
        public SelectMiningPoolByLiquidityPoolIdQuery(ulong liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            LiquidityPoolId = liquidityPoolId;
        }

        public ulong LiquidityPoolId { get; }
    }
}
