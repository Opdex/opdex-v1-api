using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.NewVaults
{
    public class NewVaultEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public Address Address { get; set; }
        public ulong TokenId { get; set; }
        public UInt256 UnassignedSupply { get; set; }
        public UInt256 VestingDuration { get; set; }
        public UInt256 ProposedSupply { get; set; }
        public ulong PledgeMinimum { get; set; }
        public ulong ProposalMinimum { get; set; }
    }
}
