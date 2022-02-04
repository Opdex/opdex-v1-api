using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;

public class RetrieveTokenWrappedByTokenIdQuery : FindQuery<TokenWrapped>
{
    public RetrieveTokenWrappedByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        TokenId = tokenId;
    }

    public ulong TokenId { get; }
}
