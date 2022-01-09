using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Domain.Models.Vaults;

/// <summary>
/// Proposal to either create or revoke a certificate in the vault, or modify the governance of a vault.
/// </summary>
public class VaultProposal : BlockAudit
{
    public VaultProposal(ulong publicId, ulong vaultId, Address creator, Address wallet, UInt256 amount, string description,
                         VaultProposalType type, VaultProposalStatus status, ulong expiration, ulong createdBlock) : base(createdBlock)
    {
        PublicId = publicId > 0 ? publicId : throw new ArgumentOutOfRangeException(nameof(publicId), "Public Id must be greater than zero.");
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault governance Id must be greater than zero.");
        Creator = creator != Address.Empty ? creator : throw new ArgumentNullException(nameof(creator), "Creator must be set.");
        Wallet = wallet != Address.Empty ? wallet : throw new ArgumentNullException(nameof(wallet), "Wallet must be set.");
        Amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        Description = description.HasValue() ? description : throw new ArgumentNullException(nameof(description), "Description must be set.");
        Type = type.IsValid() ? type : throw new ArgumentOutOfRangeException(nameof(type), "Proposal type must be valid.");
        Status = status.IsValid() ? status : throw new ArgumentOutOfRangeException(nameof(status), "Proposal status must be valid.");
        Expiration = expiration > 0 ? expiration : throw new ArgumentOutOfRangeException(nameof(expiration), "Expiration must be greater than zero.");
    }

    public VaultProposal(ulong id, ulong publicId, ulong vaultId, Address creator, Address wallet, UInt256 amount, string description,
                         VaultProposalType type, VaultProposalStatus status, ulong expiration, ulong yesAmount, ulong noAmount, ulong pledgeAmount,
                         bool approved, ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        PublicId = publicId;
        VaultId = vaultId;
        Creator = creator;
        Wallet = wallet;
        Amount = amount;
        Description = description;
        Type = type;
        Status = status;
        Expiration = expiration;
        YesAmount = yesAmount;
        NoAmount = noAmount;
        PledgeAmount = pledgeAmount;
        Approved = approved;
    }

    public ulong Id { get; }
    public ulong PublicId { get; }
    public ulong VaultId { get; }
    public Address Creator { get; }
    public Address Wallet { get; }
    public UInt256 Amount { get; }
    public string Description { get; }
    public VaultProposalType Type { get; }
    public VaultProposalStatus Status { get; private set; }
    public ulong Expiration { get; private set; }
    public ulong YesAmount { get; private set; }
    public ulong NoAmount { get; private set; }
    public ulong PledgeAmount { get; private set; }
    public bool Approved { get; private set; }

    public void Update(VaultProposalSummary summary, ulong blockHeight)
    {
        Status = summary.Status;
        Expiration = summary.Expiration;
        YesAmount = summary.YesAmount;
        NoAmount = summary.NoAmount;
        PledgeAmount = summary.PledgeAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalVoteLog log, ulong blockHeight)
    {
        Status = VaultProposalStatus.Vote;
        YesAmount = log.ProposalYesAmount;
        NoAmount = log.ProposalNoAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalPledgeLog log, ulong blockHeight)
    {
        if (log.TotalPledgeMinimumMet) Status = VaultProposalStatus.Vote;
        PledgeAmount = log.ProposalPledgeAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalWithdrawVoteLog log, ulong blockHeight)
    {
        YesAmount = log.ProposalYesAmount;
        NoAmount = log.ProposalNoAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalWithdrawPledgeLog log, ulong blockHeight)
    {
        PledgeAmount = log.ProposalPledgeAmount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(CompleteVaultProposalLog log, ulong blockHeight)
    {
        Status = VaultProposalStatus.Complete;
        Approved = log.Approved;
        SetModifiedBlock(blockHeight);
    }
}
