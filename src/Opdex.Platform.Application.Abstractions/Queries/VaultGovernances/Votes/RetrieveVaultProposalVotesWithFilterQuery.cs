using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;

/// <summary>
/// Request to retrieve votes on a proposal in a vault.
/// </summary>
public class RetrieveVaultProposalVotesWithFilterQuery : IRequest<IEnumerable<VaultProposalVote>>
{
    /// <summary>
    /// Creates a request to retrieve votes on a proposal in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="proposalId">Non-public id of the proposal.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultProposalVotesWithFilterQuery(ulong vaultId, ulong proposalId, VaultProposalVotesCursor cursor)
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
    public VaultProposalVotesCursor Cursor { get; }
}
