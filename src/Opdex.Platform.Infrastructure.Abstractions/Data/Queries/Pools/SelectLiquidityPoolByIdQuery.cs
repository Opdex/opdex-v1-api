using System;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectLiquidityPoolByIdQuery : FindQuery<LiquidityPool>
    {
        public SelectLiquidityPoolByIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
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