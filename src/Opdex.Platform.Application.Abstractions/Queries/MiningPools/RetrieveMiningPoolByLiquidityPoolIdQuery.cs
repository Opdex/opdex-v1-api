using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools
{
    public class RetrieveMiningPoolByLiquidityPoolIdQuery : FindQuery<MiningPool>
    {
        public RetrieveMiningPoolByLiquidityPoolIdQuery(ulong liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
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
