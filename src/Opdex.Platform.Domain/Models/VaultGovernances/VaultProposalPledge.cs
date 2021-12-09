using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

/// <summary>
/// The pledge position of an address for a proposal.
/// </summary>
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
    public ulong Pledge { get; private set; }
    public ulong Balance { get; private set; }

    public void UpdateBalance(ulong balance, ulong blockHeight)
    {
        Balance = balance;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalPledgeLog log, ulong blockHeight)
    {
        // Their current pledge will always be their total balance in pledge logs
        Pledge = log.PledgerAmount;
        Balance = log.PledgerAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalWithdrawPledgeLog log, ulong blockHeight)
    {
        // Only adjust the pledge if it was withdrawn from an active proposal
        if (log.PledgeWithdrawn) Pledge = log.PledgerAmount;
        Balance = log.PledgerAmount;
        SetModifiedBlock(blockHeight);
    }
}
