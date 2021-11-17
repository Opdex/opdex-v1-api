using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Markets
{
    public class MarketSnapshotDto
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public decimal Liquidity { get; set; }
        public decimal? LiquidityDailyChange { get; set; }
        public decimal Volume { get; set; }
        public StakingSnapshotDto Staking { get; set; }
        public RewardsSnapshotDto Rewards { get; set; }
        public int SnapshotType { get; set; }
        public DateTime Timestamp { get; set; }

        public void SetLiquidityDailyChange(decimal previousLiquidity)
        {
            LiquidityDailyChange = MathExtensions.PercentChange(Liquidity, previousLiquidity);
        }
    }
}
