using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using System;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;

/// <summary>
/// Get tokens with pagination and filtering.
/// </summary>
public class GetTokensWithFilterQuery : IRequest<TokensDto>
{
    /// <summary>
    /// Constructor to build a get tokens with filter query.
    /// </summary>
    /// <param name="cursor">The cursor used for filtering and pagination.</param>
    public GetTokensWithFilterQuery(TokensCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Tokens cursor must be set.");
    }

    public TokensCursor Cursor { get; }
}