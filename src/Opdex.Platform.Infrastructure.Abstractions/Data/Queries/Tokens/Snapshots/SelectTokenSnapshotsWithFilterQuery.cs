using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots
{
    public class SelectTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public SelectTokenSnapshotsWithFilterQuery(long tokenId, long marketId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            // NOTE: skip marketId check, 0 is valid for CRS or potential global token averages

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
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

            TokenId = tokenId;
            MarketId = marketId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public long TokenId { get; }
        public long MarketId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
