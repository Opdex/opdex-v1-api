using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;

/// <summary>
/// Request to retrieve proposals in a vault.
/// </summary>
public class RetrieveVaultProposalsWithFilterQuery : IRequest<IEnumerable<VaultProposal>>
{
    /// <summary>
    /// Creates a request to retrieve proposals in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultProposalsWithFilterQuery(ulong vaultId, VaultProposalsCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        if (cursor is null) throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        VaultId = vaultId;
        Cursor = cursor;
    }

    public ulong VaultId { get; }
    public VaultProposalsCursor Cursor { get; }
}
