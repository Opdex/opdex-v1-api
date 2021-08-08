using Newtonsoft.Json;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools.Snapshots
{
    public class LiquidityPoolSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public long TransactionCount { get; set; }
        public SnapshotReservesEntity Reserves { get; set; }
        public SnapshotRewardsEntity Rewards { get; set; }
        public SnapshotStakingEntity Staking { get; set; }
        public SnapshotVolumeEntity Volume { get; set; }
        public SnapshotCostEntity Cost { get; set; }

        public string Details
        {
            get => SerializeSnapshotDetails();
            set => DeserializeSnapshotDetails(value);
        }

        private string SerializeSnapshotDetails()
        {
            return JsonConvert.SerializeObject(new
            {
                Reserves,
                Rewards,
                Staking,
                Volume,
                Cost
            });
        }

        private void DeserializeSnapshotDetails(string details)
        {
            var data = JsonConvert.DeserializeObject<LiquidityPoolSnapshotDetailsEntity>(details);

            Reserves = data.Reserves;
            Rewards = data.Rewards;
            Staking = data.Staking;
            Volume = data.Volume;
            Cost = data.Cost;
        }
    }

    internal struct LiquidityPoolSnapshotDetailsEntity
    {
        public SnapshotReservesEntity Reserves;
        public SnapshotRewardsEntity Rewards;
        public SnapshotStakingEntity Staking;
        public SnapshotVolumeEntity Volume;
        public SnapshotCostEntity Cost;
    }
}
