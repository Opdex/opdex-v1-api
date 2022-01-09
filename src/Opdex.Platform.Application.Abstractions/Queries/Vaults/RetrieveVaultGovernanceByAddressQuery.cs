using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

/// <summary>
/// Retrieve a vault governance by its smart contract address.
/// </summary>
public class RetrieveVaultGovernanceByAddressQuery : FindQuery<VaultGovernance>
{
    /// <summary>
    /// Constructor to create a retrieve vault governance by address query.
    /// </summary>
    /// <param name="vault">The address of the vault contract to look up.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public RetrieveVaultGovernanceByAddressQuery(Address vault, bool findOrThrow = true) : base(findOrThrow)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
    }

    public Address Vault { get; }
}
