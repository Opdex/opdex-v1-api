using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots
{
    public class GetTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshotDto>>
    {
        public GetTokenSnapshotsWithFilterQuery(Address tokenAddress, Address marketAddress, string candleSpan, string timeSpan)
        {
            if (tokenAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenAddress));
            }

            if (marketAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(marketAddress));
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
            TokenAddress = tokenAddress;
            MarketAddress = marketAddress;
        }

        public Address TokenAddress { get; }
        public Address MarketAddress { get; }
        public SnapshotType SnapshotType { get; }
        public string TimeSpan { get; }
    }
}
