using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectTokenByIdQuery : FindQuery<Token>
    {
        public SelectTokenByIdQuery(ulong tokenId, bool findOrThrow = true)
            : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}
