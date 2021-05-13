using System;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectMiningPoolByLiquidityPoolIdQuery : IRequest<MiningPool>
    {
        public SelectMiningPoolByLiquidityPoolIdQuery(long liquidityPoolId)
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