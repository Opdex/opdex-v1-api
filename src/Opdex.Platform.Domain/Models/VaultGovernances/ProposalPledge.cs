using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

public class ProposalPledge : BlockAudit
{
    public ProposalPledge(ulong vaultGovernanceId, ulong proposalId, Address pledger, UInt256 amount, ulong createdBlock) : base(createdBlock)
    {
        VaultGovernanceId = vaultGovernanceId > 0 ? vaultGovernanceId : throw new ArgumentOutOfRangeException(nameof(vaultGovernanceId), "Vault governance id must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger must be provided.");
        Amount = amount;
    }

    public ProposalPledge(ulong id, ulong vaultGovernanceId, ulong proposalId, Address pledger, UInt256 amount,
                          ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        VaultGovernanceId = vaultGovernanceId;
        ProposalId = proposalId;
        Pledger = pledger;
        Amount = amount;
    }

    public ulong Id { get; }
    public ulong VaultGovernanceId { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public UInt256 Amount { get; }
}
