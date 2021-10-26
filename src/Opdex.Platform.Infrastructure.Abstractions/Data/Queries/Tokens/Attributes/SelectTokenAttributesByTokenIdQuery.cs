using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes
{
    /// <summary>
    /// Select token attributes based on a TokenId.
    /// </summary>
    public class SelectTokenAttributesByTokenIdQuery : IRequest<IEnumerable<TokenAttribute>>
    {
        /// <summary>
        /// Constructor to build a select token attributes by token id query.
        /// </summary>
        /// <param name="tokenId">The token id to select attributes for.</param>
        public SelectTokenAttributesByTokenIdQuery(ulong tokenId)
        {
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
        }

        public ulong TokenId { get; }
    }
}
