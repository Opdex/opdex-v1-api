using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;

/// <summary>
/// Request to retrieve pledges against proposals in a vault.
/// </summary>
public class RetrieveVaultProposalPledgesWithFilterQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Creates a request to retrieve pledges against proposals in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultProposalPledgesWithFilterQuery(ulong vaultId, VaultProposalPledgesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultProposalPledgesCursor Cursor { get; }
}
