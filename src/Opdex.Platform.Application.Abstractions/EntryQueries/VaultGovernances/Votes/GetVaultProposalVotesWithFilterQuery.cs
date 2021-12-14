using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;

/// <summary>
/// Request to retrieve votes on a proposal in a vault.
/// </summary>
public class GetVaultProposalVotesWithFilterQuery : IRequest<VaultProposalVotesDto>
{
    /// <summary>
    /// Creates a request to retrieve votes on a proposal in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="publicProposalId">Id of the proposal in the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultProposalVotesWithFilterQuery(Address vault, ulong publicProposalId, VaultProposalVotesCursor cursor)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (publicProposalId == 0) throw new ArgumentOutOfRangeException(nameof(publicProposalId), "Public proposal id must be greater than zero.");
        if (cursor is null) throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        Vault = vault;
        PublicProposalId = publicProposalId;
        Cursor = cursor;
    }

    public Address Vault { get; }
    public ulong PublicProposalId { get; }
    public VaultProposalVotesCursor Cursor { get; }
}
