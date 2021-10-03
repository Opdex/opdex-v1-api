using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools
{
    public class MiningPoolEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong LiquidityPoolId { get; set; }
        public Address Address { get; set; }
        public UInt256 RewardPerBlock { get; set; }
        public UInt256 RewardPerLpt { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}
