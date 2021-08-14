using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools.Snapshots
{
    public class SelectLiquidityPoolSnapshotWithFilterQuery : IRequest<LiquidityPoolSnapshot>
    {
        public SelectLiquidityPoolSnapshotWithFilterQuery(long liquidityPoolId, DateTime date, SnapshotType snapshotType)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "liquidityPoolId must be greater than 0.");
            }

            LiquidityPoolId = liquidityPoolId;
            Date = date;
            SnapshotType = snapshotType;
        }

        public long LiquidityPoolId { get; }
        public DateTime Date { get; }
        public SnapshotType SnapshotType { get; }
    }
}
