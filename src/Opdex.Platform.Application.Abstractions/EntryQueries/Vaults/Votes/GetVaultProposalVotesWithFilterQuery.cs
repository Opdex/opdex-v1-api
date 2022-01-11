using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Votes;

/// <summary>
/// Request to retrieve votes on proposals in a vault.
/// </summary>
public class GetVaultProposalVotesWithFilterQuery : IRequest<VaultProposalVotesDto>
{
    /// <summary>
    /// Creates a request to retrieve votes on proposals in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultProposalVotesWithFilterQuery(Address vault, VaultProposalVotesCursor cursor)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        Vault = vault;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Vault { get; }
    public VaultProposalVotesCursor Cursor { get; }
}
