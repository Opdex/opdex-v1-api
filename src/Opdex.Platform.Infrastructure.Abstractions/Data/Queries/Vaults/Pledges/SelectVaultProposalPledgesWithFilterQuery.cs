using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;

/// <summary>
/// Request to select proposal pledges from the indexed data.
/// </summary>
public class SelectVaultProposalPledgesWithFilterQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Creates a request to select proposal pledges from the indexed data.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultProposalPledgesWithFilterQuery(ulong vaultId, VaultProposalPledgesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultProposalPledgesCursor Cursor { get; }
}
