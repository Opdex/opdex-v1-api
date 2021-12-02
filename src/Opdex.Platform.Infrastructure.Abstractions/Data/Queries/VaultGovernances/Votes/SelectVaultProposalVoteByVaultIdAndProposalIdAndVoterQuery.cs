using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;

public class SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery : FindQuery<VaultProposalVote>
{
    public SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(ulong vaultId, ulong proposalId, Address voter, bool findOrThrow = true)
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
