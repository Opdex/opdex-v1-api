using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;

/// <summary>
/// Query to retrieve a vault proposal by the contract address and public proposal id.
/// </summary>
public class GetVaultProposalByVaultAddressAndPublicIdQuery : IRequest<VaultProposalDto>
{
    /// <summary>
    /// Creates a query to retrieve a vault proposal by the contract address and public proposal id.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Public proposal id stored in the vault contract.</param>
    public GetVaultProposalByVaultAddressAndPublicIdQuery(Address vault, ulong proposalId)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        Vault = vault;
        PublicProposalId = proposalId;
    }

    public Address Vault { get; }
    public ulong PublicProposalId { get; }
}
