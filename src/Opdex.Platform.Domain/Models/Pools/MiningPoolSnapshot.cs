using System;
using Opdex.Platform.Common;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class MiningPoolSnapshot
    {
        public MiningPoolSnapshot(SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            MiningWeight = "0";
            MiningUsd = 0.00m;
            RemainingRewards = "0";
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public MiningPoolSnapshot(long id, long miningPoolId, string miningWeight, decimal miningUsd, 
            string remainingRewards, SnapshotType snapshotTypeId, DateTime startDate, DateTime endDate)
        {
            Id = id;
            MiningPoolId = miningPoolId;
            MiningWeight = miningWeight;
            MiningUsd = miningUsd;
            RemainingRewards = remainingRewards;
            SnapshotType = snapshotTypeId;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public long Id { get; }
        public long MiningPoolId { get; }
        public string MiningWeight { get; }
        public decimal MiningUsd { get; }
        public string RemainingRewards { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}