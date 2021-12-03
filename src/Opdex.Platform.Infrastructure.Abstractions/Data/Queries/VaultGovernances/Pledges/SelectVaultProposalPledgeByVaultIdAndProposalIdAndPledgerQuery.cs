using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;

public class SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery : FindQuery<VaultProposalPledge>
{
    public SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(ulong vaultId, ulong proposalId, Address pledger, bool findOrThrow = true)
        : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger address must be provided.");
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
}
