using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries
{
    public class SelectTokenSummaryByMarketAndTokenIdQuery : FindQuery<TokenSummary>
    {
        public SelectTokenSummaryByMarketAndTokenIdQuery(ulong marketId, ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            TokenId = tokenId;
            MarketId = marketId;
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
    }
}
