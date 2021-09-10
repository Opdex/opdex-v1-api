using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots
{
    public class GetLiquidityPoolSnapshotsWithFilterQuery : IRequest<IEnumerable<LiquidityPoolSnapshotDto>>
    {
        public GetLiquidityPoolSnapshotsWithFilterQuery(Address liquidityPoolAddress, string candleSpan, string timeSpan)
        {
            if (liquidityPoolAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress));
            }

            timeSpan = timeSpan.HasValue() ? timeSpan : "1W";
            candleSpan = candleSpan.HasValue() ? candleSpan : "Daily";

            if (!Enum.TryParse<SnapshotType>(candleSpan, out var snapshotType) ||
                (snapshotType != SnapshotType.Daily && snapshotType != SnapshotType.Hourly))
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            SnapshotType = snapshotType;
            TimeSpan = timeSpan;
            LiquidityPoolAddress = liquidityPoolAddress;
        }

        public Address LiquidityPoolAddress { get; }
        public SnapshotType SnapshotType { get; }
        public string TimeSpan { get; }
    }
}
