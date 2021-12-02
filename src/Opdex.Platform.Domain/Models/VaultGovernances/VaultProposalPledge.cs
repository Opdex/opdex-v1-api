using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

public class VaultProposalPledge : BlockAudit
{
    public VaultProposalPledge(ulong vaultGovernanceId, ulong proposalId, Address pledger, ulong pledge, ulong balance, ulong createdBlock)
        : base(createdBlock)
    {
        VaultGovernanceId = vaultGovernanceId > 0 ? vaultGovernanceId : throw new ArgumentOutOfRangeException(nameof(vaultGovernanceId), "Vault governance id must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger must be provided.");
        Pledge = pledge > 0 ? pledge : throw new ArgumentNullException(nameof(pledge), "Pledge must be greater than zero.");
        Balance = balance;
    }

    public VaultProposalPledge(ulong id, ulong vaultGovernanceId, ulong proposalId, Address pledger, ulong pledge, ulong balance,
                               ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        VaultGovernanceId = vaultGovernanceId;
        ProposalId = proposalId;
        Pledger = pledger;
        Pledge = pledge;
        Balance = balance;
    }

    public ulong Id { get; }
    public ulong VaultGovernanceId { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public ulong Pledge { get; }
    public ulong Balance { get; }
}
