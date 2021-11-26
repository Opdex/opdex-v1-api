using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances
{
    public class MiningGovernanceEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong TokenId { get; set; }
        public Address Address { get; set; }
        public ulong NominationPeriodEnd { get; set; }
        public ulong MiningDuration { get; set; }
        public uint MiningPoolsFunded { get; set; }
        public UInt256 MiningPoolReward { get; set; }
    }
}
