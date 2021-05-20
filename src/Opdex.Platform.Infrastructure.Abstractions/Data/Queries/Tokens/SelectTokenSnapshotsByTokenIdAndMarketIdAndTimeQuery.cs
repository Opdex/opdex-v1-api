using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public SelectTokenSnapshotsByTokenIdAndMarketIdAndTimeQuery(long tokenId, long marketId, DateTime time)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }
            
            if (marketId < 1)
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