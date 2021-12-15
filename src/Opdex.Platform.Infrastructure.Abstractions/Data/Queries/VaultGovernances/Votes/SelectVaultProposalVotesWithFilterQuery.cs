using MediatR;
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
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultProposalVotesWithFilterQuery(ulong vaultId, VaultProposalVotesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultProposalVotesCursor Cursor { get; }
}
