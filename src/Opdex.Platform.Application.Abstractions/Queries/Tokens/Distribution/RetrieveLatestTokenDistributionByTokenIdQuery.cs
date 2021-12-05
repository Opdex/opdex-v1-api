using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;

public class RetrieveLatestTokenDistributionByTokenIdQuery : FindQuery<TokenDistribution>
{
    public RetrieveLatestTokenDistributionByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
    }

    public ulong TokenId { get; }
}
