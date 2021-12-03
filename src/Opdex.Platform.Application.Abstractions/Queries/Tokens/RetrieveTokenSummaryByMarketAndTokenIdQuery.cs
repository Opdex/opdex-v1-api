using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens;

public class RetrieveTokenSummaryByMarketAndTokenIdQuery : FindQuery<TokenSummary>
{
    public RetrieveTokenSummaryByMarketAndTokenIdQuery(ulong marketId, ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (tokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than 0.");
        }

        TokenId = tokenId;
        MarketId = marketId;
    }

    public ulong TokenId { get; }
    public ulong MarketId { get; }
}