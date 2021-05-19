using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectMiningPoolByLiquidityPoolIdQuery : FindQuery<MiningPool>
    {
        public SelectMiningPoolByLiquidityPoolIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }
            
            LiquidityPoolId = liquidityPoolId;
        }
        
        public long LiquidityPoolId { get; }
    }
}