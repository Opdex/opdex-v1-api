using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Wrapped;

public class SelectTokenChainByTokenIdQuery : IRequest<TokenChain>
{
    public SelectTokenChainByTokenIdQuery(ulong tokenId, bool findOrThrow = true)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        TokenId = tokenId;
        FindOrThrow = findOrThrow;
    }

    public ulong TokenId { get; }
    public bool FindOrThrow { get; }
}
