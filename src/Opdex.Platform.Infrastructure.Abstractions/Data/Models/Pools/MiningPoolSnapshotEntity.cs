using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools
{
    public class MiningPoolSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long MiningPoolId { get; set; }
        public string MiningWeight { get; set; }
        public decimal MiningUsd { get; set; }
        public string RemainingRewards { get; set; }
    }
}