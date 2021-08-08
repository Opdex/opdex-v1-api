using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots
{
    public class GetTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshotDto>>
    {
        public GetTokenSnapshotsWithFilterQuery(string tokenAddress, string marketAddress, string candleSpan, string timeSpan)
        {
            if (!tokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenAddress));
            }

            if (!marketAddress.HasValue())
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

        public string TokenAddress { get; }
        public string MarketAddress { get; }
        public SnapshotType SnapshotType { get; }
        public string TimeSpan { get; }
    }
}
