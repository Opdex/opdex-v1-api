using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

/// <summary>
/// Select a vault by the Id of the token it locks.
/// </summary>
public class SelectVaultByTokenIdQuery : FindQuery<Vault>
{
    /// <summary>
    /// Constructor to create a select vault by token id query.
    /// </summary>
    /// <param name="tokenId">The internal Id of the token to lookup the vault by.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public SelectVaultByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
    }

    public ulong TokenId { get; }
}
