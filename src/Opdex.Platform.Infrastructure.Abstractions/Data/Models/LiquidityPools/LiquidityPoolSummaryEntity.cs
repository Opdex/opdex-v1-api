using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools
{
    public class LiquidityPoolSummaryEntity : AuditEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public decimal LiquidityUsd { get; set; }
        public decimal VolumeUsd { get; set; }
        public ulong StakingWeight { get; set; }
        public ulong LockedCrs { get; set; }
        public UInt256 LockedSrc { get; set; }
    }
}
