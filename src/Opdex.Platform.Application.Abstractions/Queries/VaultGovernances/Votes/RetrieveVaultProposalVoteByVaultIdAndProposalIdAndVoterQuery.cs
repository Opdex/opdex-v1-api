using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;

/// <summary>
/// Retrieve a vote for a proposal by the vault, proposal, and voter's address.
/// </summary>
public class RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery : FindQuery<VaultProposalVote>
{
    /// <summary>
    /// Constructor to create a retrieve vault proposal vote by vault id and proposal id and voter query.
    /// </summary>
    /// <param name="vaultId">The id of the vault the proposal lives in.</param>
    /// <param name="proposalId">The internal proposalId.</param>
    /// <param name="voter">The voter's address.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(ulong vaultId, ulong proposalId, Address voter, bool findOrThrow = true)
        : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Voter = voter != Address.Empty ? voter : throw new ArgumentNullException(nameof(voter), "Voter must be provided");
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public Address Voter { get; }
}
