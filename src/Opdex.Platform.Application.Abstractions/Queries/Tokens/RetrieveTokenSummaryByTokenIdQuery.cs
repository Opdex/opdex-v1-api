using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens;

public class RetrieveTokenSummaryByTokenIdQuery : IRequest<TokenSummary>
{
    public RetrieveTokenSummaryByTokenIdQuery(ulong tokenId)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero");
        TokenId = tokenId;
    }

    public ulong TokenId { get; }
}
