using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

/// <summary>
/// Select a vault by its internal Id.
/// </summary>
public class SelectVaultGovernanceByIdQuery : FindQuery<VaultGovernance>
{
    /// <summary>
    /// Constructor to create a select vault by id query.
    /// </summary>
    /// <param name="vaultId">The internal Id of the vault to lookup.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public SelectVaultGovernanceByIdQuery(ulong vaultId, bool findOrThrow = true) : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
    }

    public ulong VaultId { get; }
}
