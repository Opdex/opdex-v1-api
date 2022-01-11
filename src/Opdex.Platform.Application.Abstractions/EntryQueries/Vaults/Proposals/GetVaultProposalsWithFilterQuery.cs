using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Proposals;

/// <summary>
/// Request to retrieve proposals in a vault.
/// </summary>
public class GetVaultProposalsWithFilterQuery : IRequest<VaultProposalsDto>
{
    /// <summary>
    /// Creates a request to retrieve proposals in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultProposalsWithFilterQuery(Address vault, VaultProposalsCursor cursor)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (cursor is null) throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        Vault = vault;
        Cursor = cursor;
    }

    public Address Vault { get; }
    public VaultProposalsCursor Cursor { get; }
}
