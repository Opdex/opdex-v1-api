using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools.Snapshots
{
    public class RetrieveLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
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

            if (snapshotType == SnapshotType.Unknown)
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
