using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Select liquidity pool snapshots by the associated liquidity pool id, a date range and snapshot type.
    /// </summary>
    public class SelectLiquidityPoolSnapshotsWithFilterQuery: IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        /// <summary>
        /// Constructor to create the select liquidity pool snapshots with filter query.
        /// </summary>
        /// <param name="poolId">The liquidity pool id of snapshots to find.</param>
        /// <param name="startDate">The start date, earliest snapshot to find.</param>
        /// <param name="endDate">The end date, latest snapshot to find.</param>
        /// <param name="snapshotType">The type of snapshots to return, hourly/daily options.</param>
        public SelectLiquidityPoolSnapshotsWithFilterQuery(ulong poolId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (poolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(poolId));
            }

            if (startDate.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(startDate));
            }

            if (endDate.Equals(default) || endDate < startDate)
            {
                throw new ArgumentOutOfRangeException(nameof(endDate));
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            PoolId = poolId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public ulong PoolId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
