using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class MarketSnapshotDto
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public decimal Liquidity { get; set; }
        public decimal? LiquidityDailyChange { get; set; }
        public decimal Volume { get; set; }
        public StakingDto Staking { get; set; }
        public RewardsDto Rewards { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void SetLiquidityDailyChange(decimal previousLiquidity)
        {
            LiquidityDailyChange = Liquidity.PercentChange(previousLiquidity);
        }
    }
}
