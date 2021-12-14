using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;

/// <summary>
/// Request to select proposal votes from the indexed data.
/// </summary>
public class SelectVaultProposalVotesWithFilterQuery : IRequest<IEnumerable<VaultProposalVote>>
{
    /// <summary>
    /// Creates a request to select proposal votes from the indexed data.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="proposalId">Non-public id of the proposal.</param>
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultProposalVotesWithFilterQuery(ulong vaultId, ulong proposalId, VaultProposalVotesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        VaultId = vaultId;
        ProposalId = proposalId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public VaultProposalVotesCursor Cursor { get; }
}
