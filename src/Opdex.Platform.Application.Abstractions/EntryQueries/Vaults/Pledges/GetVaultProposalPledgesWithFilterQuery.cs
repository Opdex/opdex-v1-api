using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;

/// <summary>
/// Request to retrieve pledges against proposals in a vault.
/// </summary>
public class GetVaultProposalPledgesWithFilterQuery : IRequest<VaultProposalPledgesDto>
{
    /// <summary>
    /// Creates a request to retrieve pledges against proposals in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultProposalPledgesWithFilterQuery(Address vault, VaultProposalPledgesCursor cursor)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        Vault = vault;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Vault { get; }
    public VaultProposalPledgesCursor Cursor { get; }
}
