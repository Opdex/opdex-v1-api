using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

public class VaultProposal : BlockAudit
{
    public VaultProposal(ulong publicId, ulong vaultGovernanceId, Address creator, Address wallet, UInt256 amount, string description,
                         VaultProposalType type, VaultProposalStatus status, ulong expiration, ulong createdBlock) : base(createdBlock)
    {
        PublicId = publicId > 0 ? publicId : throw new ArgumentOutOfRangeException(nameof(publicId), "Public Id must be greater than zero.");
        VaultGovernanceId = vaultGovernanceId > 0 ? vaultGovernanceId : throw new ArgumentOutOfRangeException(nameof(vaultGovernanceId), "Vault governance Id must be greater than zero.");
        Creator = creator != Address.Empty ? creator : throw new ArgumentNullException(nameof(creator), "Creator must be set.");
        Wallet = wallet != Address.Empty ? wallet : throw new ArgumentNullException(nameof(wallet), "Wallet must be set.");
        Amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        Description = description.HasValue() ? description : throw new ArgumentNullException(nameof(description), "Description must be set.");
        Type = type.IsValid() ? type : throw new ArgumentOutOfRangeException(nameof(type), "Proposal type must be valid.");
        Status = status.IsValid() ? status : throw new ArgumentOutOfRangeException(nameof(status), "Proposal status must be valid.");
        Expiration = expiration > 0 ? expiration : throw new ArgumentOutOfRangeException(nameof(expiration), "Expiration must be greater than zero.");
    }

    public VaultProposal(ulong id, ulong publicId, ulong vaultGovernanceId, Address creator, Address wallet, UInt256 amount,  string description,
                         VaultProposalType type, VaultProposalStatus status, ulong expiration, ulong yesAmount, ulong noAmount, ulong pledgeAmount,
                         ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        PublicId = publicId;
        VaultGovernanceId = vaultGovernanceId;
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
    }

    public ulong Id { get; }
    public ulong PublicId { get; }
    public ulong VaultGovernanceId { get; }
    public Address Creator { get; }
    public Address Wallet { get; }
    public UInt256 Amount { get; }
    public string Description { get; }
    public VaultProposalType Type { get; }
    public VaultProposalStatus Status { get; }
    public ulong Expiration { get; }
    public ulong YesAmount { get; }
    public ulong NoAmount { get; }
    public ulong PledgeAmount { get; }
}
