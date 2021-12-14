using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;

/// <summary>
/// Request to retrieve pledges against a proposal in a vault.
/// </summary>
public class GetVaultProposalPledgesWithFilterQuery : IRequest<VaultProposalPledgesDto>
{
    /// <summary>
    /// Creates a request to retrieve pledges against a proposal in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="publicProposalId">Id of the proposal in the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultProposalPledgesWithFilterQuery(Address vault, ulong publicProposalId, VaultProposalPledgesCursor cursor)
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
    public VaultProposalPledgesCursor Cursor { get; }
}
