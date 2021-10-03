using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Select a single liquidity pool snapshot of specified type. Returns a matching record by dateTime and type from the database else the most
    /// recent record by type. If nothing is found, will return a new snapshot instance.
    /// </summary>
    public class SelectLiquidityPoolSnapshotWithFilterQuery : IRequest<LiquidityPoolSnapshot>
    {
        /// <summary>
        /// Create the select liquidity pool snapshot with filter query.
        /// </summary>
        /// <param name="liquidityPoolId">The liquidity pool id to get the snapshot of.</param>
        /// <param name="dateTime">The date to get the snapshot of.</param>
        /// <param name="snapshotType"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public SelectLiquidityPoolSnapshotWithFilterQuery(ulong liquidityPoolId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "liquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public ulong LiquidityPoolId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}
