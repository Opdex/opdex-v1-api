using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots;

/// <summary>
/// Select snapshots for a given liquidity pool.
/// </summary>
public class SelectLiquidityPoolSnapshotsWithFilterQuery: IRequest<IEnumerable<LiquidityPoolSnapshot>>
{
    /// <summary>
    /// Constructor to create the select liquidity pool snapshots with filter query.
    /// </summary>
    /// <param name="liquidityPoolId">The liquidity pool id of snapshots to find.</param>
    /// <param name="cursor">The snapshot cursor filter.</param>
    public SelectLiquidityPoolSnapshotsWithFilterQuery(ulong liquidityPoolId, SnapshotCursor cursor)
    {
        LiquidityPoolId = liquidityPoolId > 0 ? liquidityPoolId : throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public ulong LiquidityPoolId { get; }
    public SnapshotCursor Cursor { get; }
}