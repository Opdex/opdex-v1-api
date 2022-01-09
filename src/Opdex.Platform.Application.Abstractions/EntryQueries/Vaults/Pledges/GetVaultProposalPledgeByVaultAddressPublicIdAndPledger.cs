using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;

/// <summary>
/// Query to retrieve a vault proposal pledge by the contract address, public proposal id and pledger address.
/// </summary>
public class GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery : IRequest<VaultProposalPledgeDto>
{
    /// <summary>
    /// Creates a query to retrieve a vault proposal pledge by the contract address, public proposal id and pledger address.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Public proposal id stored in the vault contract.</param>
    /// <param name="pledger">Address of the pledger.</param>
    public GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(Address vault, ulong proposalId, Address pledger)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        if (pledger == Address.Empty) throw new ArgumentNullException(nameof(pledger), "Pledger address must be set.");
        Vault = vault;
        PublicProposalId = proposalId;
        Pledger = pledger;
    }

    public Address Vault { get; }
    public ulong PublicProposalId { get; }
    public Address Pledger { get; }
}
