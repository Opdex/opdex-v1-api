using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using System;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class MarketSnapshotDto
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public decimal Liquidity { get; set; }
        public decimal LiquidityDailyChange { get; set; }
        public decimal Volume { get; set; }
        public StakingDto Staking { get; set; }
        public RewardsDto Rewards { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void SetLiquidityDailyChange(decimal previousLiquidity)
        {
            if (previousLiquidity <= 0)
            {
                LiquidityDailyChange = 0.00m;
                return;
            }

            var usdDailyChange = (Liquidity - previousLiquidity) / previousLiquidity * 100;

            LiquidityDailyChange = Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
        }
    }
}
