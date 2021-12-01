using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.NewVaults
{
    public class NewVaultProposal : BlockAudit
    {
        public NewVaultProposal(ulong id, UInt256 amount, Address wallet, string description, VaultProposalType type, VaultProposalStatus status,
                                ulong expiration, ulong createdBlock) : base(createdBlock)
        {
            // Not auto increment, so validate and set, use the smart contract's proposal Id's
            Id = id > 0 ? id : throw new ArgumentOutOfRangeException(nameof(id), "Proposal Id must be greater than zero.");
            Amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            Wallet = wallet != Address.Empty ? wallet : throw new ArgumentNullException(nameof(wallet), "Wallet must be set.");
            Description = description.HasValue() ? description : throw new ArgumentNullException(nameof(description), "Description must be set.");
            Type = type.IsValid() ? type : throw new ArgumentOutOfRangeException(nameof(type), "Proposal type must be valid.");
            Status = status.IsValid() ? status : throw new ArgumentOutOfRangeException(nameof(status), "Proposal status must be valid.");
            Expiration = expiration > 0 ? expiration : throw new ArgumentOutOfRangeException(nameof(expiration), "Expiration must be greater than zero.");
        }

        public NewVaultProposal(ulong id, UInt256 amount, Address wallet, string description, VaultProposalType type, VaultProposalStatus status,
                                ulong expiration, ulong yesAmount, ulong noAmount, ulong pledgeAmount, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Amount = amount;
            Wallet = wallet;
            Description = description;
            Type = type;
            Status = status;
            Expiration = expiration;
            YesAmount = yesAmount;
            NoAmount = noAmount;
            PledgeAmount = pledgeAmount;
        }

        public ulong Id { get; }
        public UInt256 Amount { get; }
        public Address Wallet { get; }
        public string Description { get; }
        public VaultProposalType Type { get; }
        public VaultProposalStatus Status { get; }
        public ulong Expiration { get; }
        public ulong YesAmount { get; }
        public ulong NoAmount { get; }
        public ulong PledgeAmount { get; }
    }
}
