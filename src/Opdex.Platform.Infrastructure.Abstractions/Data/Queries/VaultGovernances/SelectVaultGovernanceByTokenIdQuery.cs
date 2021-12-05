using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;

/// <summary>
/// Select a vault governance by the Id of the token it locks.
/// </summary>
public class SelectVaultGovernanceByTokenIdQuery : FindQuery<VaultGovernance>
{
    /// <summary>
    /// Constructor to create a select vault governance by token id query.
    /// </summary>
    /// <param name="tokenId">The internal Id of the token to lookup the vault by.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public SelectVaultGovernanceByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
    }

    public ulong TokenId { get; }
}
