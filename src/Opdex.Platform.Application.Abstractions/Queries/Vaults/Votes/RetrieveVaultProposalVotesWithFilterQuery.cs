using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;

/// <summary>
/// Request to retrieve votes on proposals in a vault.
/// </summary>
public class RetrieveVaultProposalVotesWithFilterQuery : IRequest<IEnumerable<VaultProposalVote>>
{
    /// <summary>
    /// Creates a request to retrieve votes on proposals in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultProposalVotesWithFilterQuery(ulong vaultId, VaultProposalVotesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultProposalVotesCursor Cursor { get; }
}
