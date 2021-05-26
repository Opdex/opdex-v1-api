using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveMiningPoolByLiquidityPoolIdQuery : FindQuery<MiningPool>
    {
        public RetrieveMiningPoolByLiquidityPoolIdQuery(long liquidityPoolId, bool findOrThrow = true) : base(findOrThrow)
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