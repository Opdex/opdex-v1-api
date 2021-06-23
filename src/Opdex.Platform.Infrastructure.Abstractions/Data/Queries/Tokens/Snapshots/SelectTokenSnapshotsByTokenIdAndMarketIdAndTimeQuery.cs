using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots
{
    public class SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(long tokenId, long marketId, DateTime time)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            // Todo: Think and fix
            // MarketId should be required for all tokens that are not CRS
            // CRS uses market 0 as a global market.
            if (marketId < 1 && tokenId > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            if (time.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(time));
            }

            TokenId = tokenId;
            MarketId = marketId;
            Time = time;
        }

        public long TokenId { get; }
        public long MarketId { get; }
        public DateTime Time { get; }
    }
}