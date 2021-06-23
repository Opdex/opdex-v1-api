using System;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots
{
    public class RetrieveTokenSnapshotWithFilterQuery : IRequest<TokenSnapshot>
    {
        public RetrieveTokenSnapshotWithFilterQuery(long tokenId, long marketId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            if (snapshotType == SnapshotType.Unknown)
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