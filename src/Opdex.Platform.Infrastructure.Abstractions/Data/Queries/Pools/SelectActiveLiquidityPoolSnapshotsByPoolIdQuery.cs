using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectActiveLiquidityPoolSnapshotsByPoolIdQuery: IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        public SelectActiveLiquidityPoolSnapshotsByPoolIdQuery(long poolId, DateTime time)
        {
            if (poolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(poolId));
            }

            PoolId = poolId;
            Time = time;
        }
        
        public long PoolId { get; }
        public DateTime Time { get; }
    }
}