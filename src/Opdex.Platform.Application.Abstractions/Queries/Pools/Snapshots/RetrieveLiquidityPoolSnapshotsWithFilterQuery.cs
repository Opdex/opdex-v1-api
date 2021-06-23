using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Pools.Snapshots;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots
{
    public class RetrieveLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        public RetrieveLiquidityPoolSnapshotsWithFilterQuery(long liquidityPoolId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
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