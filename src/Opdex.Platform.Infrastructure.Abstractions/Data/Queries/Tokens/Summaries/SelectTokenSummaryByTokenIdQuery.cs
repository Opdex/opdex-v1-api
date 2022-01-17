using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;

public class SelectTokenSummaryByTokenIdQuery : IRequest<TokenSummary>
{
    public SelectTokenSummaryByTokenIdQuery(ulong tokenId)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
        TokenId = tokenId;
    }

    public ulong TokenId { get; }
}
