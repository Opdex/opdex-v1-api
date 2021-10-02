using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots
{
    /// <summary>
    /// Retrieve liquidity pool snapshots by the associated liquidity pool id, a date range and snapshot type.
    /// </summary>
    public class RetrieveLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        /// <summary>
        /// Constructor to create the retrieve liquidity pool snapshots with filter query.
        /// </summary>
        /// <param name="liquidityPoolId">The liquidity pool id of snapshots to find.</param>
        /// <param name="startDate">The start date, earliest snapshot to find.</param>
        /// <param name="endDate">The end date, latest snapshot to find.</param>
        /// <param name="snapshotType">The type of snapshots to return, hourly/daily options.</param>
        public RetrieveLiquidityPoolSnapshotsWithFilterQuery(long liquidityPoolId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
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

            LiquidityPoolId = liquidityPoolId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public long LiquidityPoolId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
