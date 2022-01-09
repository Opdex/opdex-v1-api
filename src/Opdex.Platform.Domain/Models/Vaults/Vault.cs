using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Domain.Models.Vaults;

/// <summary>
/// Vault that CRS holders can vote on proposals for the allocation of vested certificates. Certificates entitle the holder to Opdex governance upon completion of the vesting period.
/// </summary>
public class Vault : BlockAudit
{
    public Vault(Address address, ulong tokenId, ulong vestingDuration, ulong totalPledgeMinimum, ulong totalVoteMinimum, ulong createdBlock)
        : base(createdBlock)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
        VestingDuration = vestingDuration > 0 ? vestingDuration : throw new ArgumentOutOfRangeException(nameof(vestingDuration), "Vesting duration must be greater than zero.");
        TotalPledgeMinimum = totalPledgeMinimum;
        TotalVoteMinimum = totalVoteMinimum;
    }

    public Vault(ulong id, Address address, ulong tokenId, UInt256 unassignedSupply, ulong vestingDuration, UInt256 proposedSupply,
                           ulong totalPledgeMinimum, ulong totalVoteMinimum, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        Address = address;
        TokenId = tokenId;
        UnassignedSupply = unassignedSupply;
        VestingDuration = vestingDuration;
        ProposedSupply = proposedSupply;
        TotalPledgeMinimum = totalPledgeMinimum;
        TotalVoteMinimum = totalVoteMinimum;
    }

    public ulong Id { get; }
    public Address Address { get; }
    public ulong TokenId { get; }
    public ulong VestingDuration { get; }
    public UInt256 UnassignedSupply { get; private set; }
    public UInt256 ProposedSupply { get; private set; }
    public ulong TotalPledgeMinimum { get; private set; }
    public ulong TotalVoteMinimum { get; private set; }

    public void Update(VaultContractSummary summary)
    {
        if (summary is null) throw new ArgumentNullException(nameof(summary));

        if (summary.UnassignedSupply.HasValue) UnassignedSupply = summary.UnassignedSupply.Value;
        if (summary.ProposedSupply.HasValue) ProposedSupply = summary.ProposedSupply.Value;
        if (summary.TotalPledgeMinimum.HasValue) TotalPledgeMinimum = summary.TotalPledgeMinimum.Value;
        if (summary.TotalVoteMinimum.HasValue) TotalVoteMinimum = summary.TotalVoteMinimum.Value;
        SetModifiedBlock(summary.BlockHeight);
    }

    public void Update(CompleteVaultProposalLog log, VaultProposal proposal, ulong blockHeight)
    {
        var isPledgeMinimum = proposal.Type == VaultProposalType.TotalPledgeMinimum;
        var isVoteMinimum = proposal.Type == VaultProposalType.TotalVoteMinimum;

        if (!log.Approved || (!isPledgeMinimum && !isVoteMinimum))
        {
            return;
        }

        if (isPledgeMinimum) TotalPledgeMinimum = (ulong)proposal.Amount;
        if (isVoteMinimum) TotalVoteMinimum = (ulong)proposal.Amount;

        SetModifiedBlock(blockHeight);
    }
}
