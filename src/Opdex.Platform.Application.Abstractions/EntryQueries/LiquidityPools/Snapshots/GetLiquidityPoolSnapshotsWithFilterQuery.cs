using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Get snapshot data for a given liquidity pool.
    /// </summary>
    public class GetLiquidityPoolSnapshotsWithFilterQuery : IRequest<LiquidityPoolSnapshotsDto>
    {
        /// <summary>
        /// Creates a request to retrieve snapshot data for a given liquidity pool.
        /// </summary>
        /// <param name="liquidityPool">The address of the liquidity pool.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public GetLiquidityPoolSnapshotsWithFilterQuery(Address liquidityPool, SnapshotCursor cursor)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool address must not be empty.");
            }

            LiquidityPool = liquidityPool;
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public Address LiquidityPool { get; }
        public SnapshotCursor Cursor { get; }
    }
}
