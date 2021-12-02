using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

public class VaultGovernanceContractSummary
{
    public VaultGovernanceContractSummary(ulong blockHeight)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
    public UInt256? UnassignedSupply { get; private set; }
    public UInt256? ProposedSupply { get; private set; }
    public ulong? PledgeMinimum { get; private set; }
    public ulong? ProposalMinimum { get; private set; }

    public void SetUnassignedSupply(SmartContractMethodParameter unassignedSupplyParameter)
    {
        if (unassignedSupplyParameter is null) throw new ArgumentNullException(nameof(unassignedSupplyParameter));
        UnassignedSupply = unassignedSupplyParameter.Parse<UInt256>();
    }

    public void SetProposedSupply(SmartContractMethodParameter proposedSupplyParameter)
    {
        if (proposedSupplyParameter is null) throw new ArgumentNullException(nameof(proposedSupplyParameter));
        ProposedSupply = proposedSupplyParameter.Parse<UInt256>();
    }

    public void SetPledgeMinimum(SmartContractMethodParameter pledgeMinimum)
    {
        if (pledgeMinimum is null) throw new ArgumentNullException(nameof(pledgeMinimum));
        PledgeMinimum = pledgeMinimum.Parse<ulong>();
    }

    public void SetProposalMinimum(SmartContractMethodParameter proposalMinimum)
    {
        if (proposalMinimum is null) throw new ArgumentNullException(nameof(proposalMinimum));
        ProposalMinimum = proposalMinimum.Parse<ulong>();
    }
}
