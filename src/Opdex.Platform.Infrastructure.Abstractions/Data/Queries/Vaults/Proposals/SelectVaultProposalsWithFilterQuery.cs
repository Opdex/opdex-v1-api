using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;

/// <summary>
/// Request to select proposals from the indexed data.
/// </summary>
public class SelectVaultProposalsWithFilterQuery : IRequest<IEnumerable<VaultProposal>>
{
    /// <summary>
    /// Creates a request to select proposals from the indexed data.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultProposalsWithFilterQuery(ulong vaultId, VaultProposalsCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultProposalsCursor Cursor { get; }
}
