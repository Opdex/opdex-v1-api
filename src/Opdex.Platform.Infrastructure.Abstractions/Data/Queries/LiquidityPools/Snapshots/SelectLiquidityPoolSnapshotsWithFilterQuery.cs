using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots
{
    public class SelectLiquidityPoolSnapshotsWithFilterQuery: IRequest<IEnumerable<LiquidityPoolSnapshot>>
    {
        public SelectLiquidityPoolSnapshotsWithFilterQuery(long poolId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
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

        public long PoolId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
