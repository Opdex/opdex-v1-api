using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes
{
    public class SelectTokenAttributesByTokenIdQuery : IRequest<IEnumerable<TokenAttribute>>
    {
        public SelectTokenAttributesByTokenIdQuery(ulong tokenId)
        {
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
        }

        public ulong TokenId { get; }
    }
}
