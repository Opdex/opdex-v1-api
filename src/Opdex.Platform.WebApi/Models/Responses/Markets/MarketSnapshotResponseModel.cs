using Opdex.Platform.WebApi.Models.Responses.Pools;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Markets
{
    public class MarketSnapshotResponseModel
    {
        public decimal Liquidity { get; set; }
        public decimal? LiquidityDailyChange { get; set; }
        public decimal Volume { get; set; }
        public StakingResponseModel Staking { get; set; }
        public RewardsResponseModel Rewards { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
