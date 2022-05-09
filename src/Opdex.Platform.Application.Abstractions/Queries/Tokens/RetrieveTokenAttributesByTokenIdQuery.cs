using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens;

/// <summary>
/// Retrieve token attributes based on a token id.
/// </summary>
public class RetrieveTokenAttributesByTokenIdQuery : IRequest<IEnumerable<TokenAttribute>>
{
    /// <summary>
    /// Constructor to build a retrieve token attributes by token id query.
    /// </summary>
    /// <param name="tokenId">The token id to select attributes for.</param>
    public RetrieveTokenAttributesByTokenIdQuery(ulong tokenId)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
    }

    public ulong TokenId { get; }
}
