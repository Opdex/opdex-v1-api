using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Domain.Models.Vaults;

/// <summary>
/// The pledge position of an address for a proposal.
/// </summary>
public class VaultProposalPledge : BlockAudit
{
    public VaultProposalPledge(ulong vaultId, ulong proposalId, Address pledger, ulong pledge, ulong balance, ulong createdBlock)
        : base(createdBlock)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger must be provided.");
        Pledge = pledge > 0 ? pledge : throw new ArgumentOutOfRangeException(nameof(pledge), "Pledge must be greater than zero.");
        Balance = balance;
    }

    public VaultProposalPledge(ulong id, ulong vaultId, ulong proposalId, Address pledger, ulong pledge, ulong balance,
                               ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        VaultId = vaultId;
        ProposalId = proposalId;
        Pledger = pledger;
        Pledge = pledge;
        Balance = balance;
    }

    public ulong Id { get; }
    public ulong VaultId { get; }
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
