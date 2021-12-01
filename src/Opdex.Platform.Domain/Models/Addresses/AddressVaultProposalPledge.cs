using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressVaultProposalPledge : BlockAudit
    {
        public AddressVaultProposalPledge(ulong vaultId, ulong proposalId, Address pledger, UInt256 amount, ulong createdBlock) : base(createdBlock)
        {
            VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
            ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
            Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger must be provided.");
            Amount = amount;
        }

        public AddressVaultProposalPledge(ulong id, ulong vaultId, ulong proposalId, Address pledger, UInt256 amount,
                                          ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            VaultId = vaultId;
            ProposalId = proposalId;
            Pledger = pledger;
            Amount = amount;
        }

        public ulong Id { get; }
        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Pledger { get; }
        public UInt256 Amount { get; }
    }
}
