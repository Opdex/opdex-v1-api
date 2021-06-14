using System;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Pools.Snapshot;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots
{
    public class RetrieveLiquidityPoolSnapshotWithFilterQuery : IRequest<LiquidityPoolSnapshot>
    {
        public RetrieveLiquidityPoolSnapshotWithFilterQuery(long liquidityPoolId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "liquidityPoolId must be greater than 0.");
            }

            if (snapshotType == SnapshotType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            LiquidityPoolId = liquidityPoolId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public long LiquidityPoolId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}