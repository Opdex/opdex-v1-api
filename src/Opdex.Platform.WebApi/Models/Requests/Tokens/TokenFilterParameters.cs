using NJsonSchema.Annotations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Tokens;

public sealed class TokenFilterParameters : FilterParameters<TokensCursor>
{
    public TokenFilterParameters()
    {
        Tokens = new List<Address>();
        IncludeZeroLiquidity = true;
    }

    /// <summary>
    /// The order to sort records by.
    /// </summary>
    /// <example>Symbol</example>
    public TokenOrderByType OrderBy { get; set; }

    /// <summary>
    /// The type of token to filter for, liquidity pool tokens or not.
    /// </summary>
    /// <example>Provisional</example>
    public TokenAttributeFilter TokenType { get; set; }

    /// <summary>
    /// Tokens to filter specifically for.
    /// </summary>
    /// <example>[ "tFPedNjm3q8N9HD7wSVTNK5Kvw96332P1o", "tHb7w3dpzq9d8uBVYRrYrqHfBdoMZXqTzG" ]</example>
    [NotNull]
    public IEnumerable<Address> Tokens { get; set; }

    /// <summary>
    /// A generic keyword search against token addresses, names and ticker symbols.
    /// </summary>
    /// <example>BTC</example>
    [NotNull]
    public string Keyword { get; set; }

    /// <summary>
    /// Includes tokens with no liquidity, only if true. Default true.
    /// </summary>
    /// <example>true</example>
    public bool IncludeZeroLiquidity { get; set; }

    /// <inheritdoc />
    protected override TokensCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new TokensCursor(Keyword, Tokens, TokenType, IncludeZeroLiquidity, OrderBy, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = TokensCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
