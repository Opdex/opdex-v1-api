using System;

namespace Opdex.Platform.Domain.Models
{
    public class LiquidityPoolSnapshot
    {
        public LiquidityPoolSnapshot(long id, long poolId, long transactionCount, string reserveCrs, string reserveSrc, decimal reserveUsd,
            string volumeCrs, string volumeSrc, decimal volumeUsd, string stakingWeight, decimal stakingUsd, SnapshotType snapshotType,
            DateTime startDate, DateTime endDate)
        {
            Id = id;
            PoolId = poolId;
            TransactionCount = transactionCount;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
            ReserveUsd = reserveUsd;
            VolumeCrs = volumeCrs;
            VolumeSrc = volumeSrc;
            VolumeUsd = volumeUsd;
            StakingWeight = stakingWeight;
            StakingUsd = stakingUsd;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public long Id { get; }
        public long PoolId { get; }
        public long TransactionCount { get; }
        public string ReserveCrs { get; }
        public string ReserveSrc { get; }
        public decimal ReserveUsd { get; }
        public string VolumeCrs { get; }
        public string VolumeSrc { get; }
        public decimal VolumeUsd { get; }
        public string StakingWeight { get; }
        public decimal StakingUsd { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}