using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        public RetrieveActiveLiquidityPoolSnapshotsByPoolIdQuery(long marketId, DateTime time)
        {
            PoolId = marketId;
            Time = time;
        }
        
        public long PoolId { get; }
        public DateTime Time { get; }
    }
}