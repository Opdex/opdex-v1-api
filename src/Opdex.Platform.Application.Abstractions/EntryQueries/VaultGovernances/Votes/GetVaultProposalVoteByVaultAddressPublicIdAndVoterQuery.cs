using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;

/// <summary>
/// Query to retrieve a vault proposal vote by the contract address, public proposal id and voter address.
/// </summary>
public class GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery : IRequest<VaultProposalVoteDto>
{
    /// <summary>
    /// Creates a query to retrieve a vault proposal vote by the contract address, public proposal id and voter address.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Public proposal id stored in the vault contract.</param>
    /// <param name="voter">Address of the voter.</param>
    public GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(Address vault, ulong proposalId, Address voter)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        if (voter == Address.Empty) throw new ArgumentNullException(nameof(voter), "Voter address must be set.");
        Vault = vault;
        PublicProposalId = proposalId;
        Voter = voter;
    }

    public Address Vault { get; }
    public ulong PublicProposalId { get; }
    public Address Voter { get; }
}
