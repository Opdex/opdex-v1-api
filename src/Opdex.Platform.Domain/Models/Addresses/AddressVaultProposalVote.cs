using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressVaultProposalVote : BlockAudit
    {
        public AddressVaultProposalVote(ulong vaultId, ulong proposalId, Address voter, UInt256 amount, bool inFavor, ulong createdBlock) : base(createdBlock)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
            ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
            Voter = voter != Address.Empty ? voter : throw new ArgumentNullException(nameof(voter), "Voter must be provided.");
            Amount = amount;
            InFavor = inFavor;
        }

        public AddressVaultProposalVote(ulong id, ulong vaultId, ulong proposalId, Address voter, UInt256 amount, bool inFavor,
                                        ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            VaultId = vaultId;
            ProposalId = proposalId;
            Voter = voter;
            Amount = amount;
            InFavor = inFavor;
        }

        public ulong Id { get; }
        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Voter { get; }
        public UInt256 Amount { get; }
        public bool InFavor { get; }
    }
}
