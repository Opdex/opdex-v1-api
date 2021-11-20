using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Newtonsoft.Json;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets
{
    public class MarketSnapshotEntity : SnapshotEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public OhlcEntity<decimal> Liquidity { get; set; }
        public decimal Volume { get; set; }
        public SnapshotStakingEntity Staking { get; set; }
        public SnapshotRewardsEntity Rewards { get; set; }

        public string Details
        {
            get => SerializeSnapshotDetails();
            set => DeserializeSnapshotDetails(value);
        }

        private string SerializeSnapshotDetails()
        {
            return JsonConvert.SerializeObject(new
            {
                Liquidity,
                Volume,
                Rewards,
                Staking
            });
        }

        private void DeserializeSnapshotDetails(string details)
        {
            var data = JsonConvert.DeserializeObject<MarketSnapshotDetailsEntity>(details);

            Liquidity = data.Liquidity;
            Volume = data.Volume;
            Staking = data.Staking;
            Rewards = data.Rewards;
        }
    }

    internal struct MarketSnapshotDetailsEntity
    {
        public OhlcEntity<decimal> Liquidity;
        public decimal Volume;
        public SnapshotStakingEntity Staking;
        public SnapshotRewardsEntity Rewards;
    }
}
