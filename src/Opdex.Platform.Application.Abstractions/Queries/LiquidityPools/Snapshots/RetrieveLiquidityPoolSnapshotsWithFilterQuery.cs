using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Retrieve snapshots for the provided liquidity pool.
    /// </summary>
    public class RetrieveLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        /// <summary>
        /// Constructor to create the retrieve liquidity pool snapshots with filter query.
        /// </summary>
        /// <param name="liquidityPoolId">The liquidity pool id of snapshots to find.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public RetrieveLiquidityPoolSnapshotsWithFilterQuery(ulong liquidityPoolId, SnapshotCursor cursor)
        {
            LiquidityPoolId = liquidityPoolId > 0 ? liquidityPoolId : throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public ulong LiquidityPoolId { get; }
        public SnapshotCursor Cursor { get; }
    }
}
