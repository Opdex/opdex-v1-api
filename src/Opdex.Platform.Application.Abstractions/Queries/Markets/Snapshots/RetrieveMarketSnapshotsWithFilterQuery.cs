using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots
{
    public class RetrieveMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        public RetrieveMarketSnapshotsWithFilterQuery(long marketId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            if (snapshotType == SnapshotType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            MarketId = marketId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public long MarketId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
