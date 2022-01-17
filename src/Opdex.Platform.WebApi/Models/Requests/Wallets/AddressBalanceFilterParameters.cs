using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets;

public sealed class AddressBalanceFilterParameters : FilterParameters<AddressBalancesCursor>
{
    public AddressBalanceFilterParameters()
    {
        Tokens = new List<Address>();
        TokenAttributes = new List<TokenAttributeFilter>();
    }

    /// <summary>
    /// Specific tokens to lookup.
    /// </summary>
    /// <example>[ "tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3", "tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt" ]</example>
    public IEnumerable<Address> Tokens { get; set; }

    /// <summary>
    /// The types of token to filter balances by.
    /// </summary>
    /// <example>Provisional</example>
    public IEnumerable<TokenAttributeFilter> TokenAttributes { get; set; }

    /// <summary>
    /// Includes zero balances if true, otherwise filters out zero balances if false. Default false.
    /// </summary>
    /// <example>true</example>
    public bool IncludeZeroBalances { get; set; }

    /// <inheritdoc />
    protected override AddressBalancesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new AddressBalancesCursor(Tokens, TokenAttributes, IncludeZeroBalances, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        AddressBalancesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
