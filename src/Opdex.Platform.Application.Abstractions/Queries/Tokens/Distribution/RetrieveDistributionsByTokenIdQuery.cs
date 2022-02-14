using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;

public class RetrieveDistributionsByTokenIdQuery : IRequest<IEnumerable<TokenDistribution>>
{
    public RetrieveDistributionsByTokenIdQuery(ulong tokenId)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
    }

    public ulong TokenId { get; }
}
