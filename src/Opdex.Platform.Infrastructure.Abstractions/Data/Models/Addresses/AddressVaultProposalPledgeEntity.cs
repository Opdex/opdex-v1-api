using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressVaultProposalPledgeEntity
    {
        public ulong Id { get; set; }
        public ulong VaultId { get; set; }
        public ulong ProposalId { get; set; }
        public Address Pledger { get; set; }
        public UInt256 Amount { get; set; }
    }
}
