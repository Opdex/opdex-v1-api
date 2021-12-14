using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;

/// <summary>
/// Request to retrieve pledges against a proposal in a vault.
/// </summary>
public class RetrieveVaultProposalPledgesWithFilterQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Creates a request to retrieve pledges against a proposal in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="proposalId">Non-public id of the proposal.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultProposalPledgesWithFilterQuery(ulong vaultId, ulong proposalId, VaultProposalPledgesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        if (cursor is null) throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        VaultId = vaultId;
        ProposalId = proposalId;
        Cursor = cursor;
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public VaultProposalPledgesCursor Cursor { get; }
}
