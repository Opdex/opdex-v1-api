using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools
{
    public class LiquidityPoolSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public long TransactionCount { get; set; }
        public string ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
        public decimal ReserveUsd { get; set; }
        public string VolumeCrs { get; set; }
        public string VolumeSrc { get; set; }
        public decimal VolumeUsd { get; set; }
        public string StakingWeight { get; set; }
        public decimal StakingUsd { get; set; }
        public decimal ProviderRewards { get; set; }
        public decimal StakerRewards { get; set; }
    }
}