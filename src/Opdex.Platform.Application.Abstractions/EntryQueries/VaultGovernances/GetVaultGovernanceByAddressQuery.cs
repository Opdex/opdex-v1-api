using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;

/// <summary>
/// Query to retrieve a vault by the contract address.
/// </summary>
public class GetVaultGovernanceByAddressQuery : IRequest<VaultGovernanceDto>
{
    /// <summary>
    /// Creates a query to retrieve a vault by the contract address.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    public GetVaultGovernanceByAddressQuery(Address vault)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        Vault = vault;
    }

    public Address Vault { get; }
}