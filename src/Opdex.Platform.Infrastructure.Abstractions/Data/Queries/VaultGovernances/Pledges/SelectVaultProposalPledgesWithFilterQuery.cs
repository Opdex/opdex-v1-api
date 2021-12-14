using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;

/// <summary>
/// Request to select proposal pledges from the indexed data.
/// </summary>
public class SelectVaultProposalPledgesWithFilterQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Creates a request to select proposal pledges from the indexed data.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="proposalId">Non-public id of the proposal.</param>
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultProposalPledgesWithFilterQuery(ulong vaultId, ulong proposalId, VaultProposalPledgesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        VaultId = vaultId;
        ProposalId = proposalId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public VaultProposalPledgesCursor Cursor { get; }
}
