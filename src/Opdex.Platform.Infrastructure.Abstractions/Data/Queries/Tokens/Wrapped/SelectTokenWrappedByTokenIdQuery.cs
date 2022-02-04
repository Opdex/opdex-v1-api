using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;

public class SelectTokenWrappedByTokenIdQuery : FindQuery<TokenWrapped>
{
    public SelectTokenWrappedByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        TokenId = tokenId;
    }

    public ulong TokenId { get; }
}
