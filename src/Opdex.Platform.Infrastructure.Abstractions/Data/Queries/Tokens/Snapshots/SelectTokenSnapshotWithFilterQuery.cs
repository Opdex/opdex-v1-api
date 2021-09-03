using System;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots
{
    public class SelectTokenSnapshotWithFilterQuery : IRequest<TokenSnapshot>
    {
        public SelectTokenSnapshotWithFilterQuery(long tokenId, long marketId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            TokenId = tokenId;
            MarketId = marketId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public long TokenId { get; }
        public long MarketId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}
