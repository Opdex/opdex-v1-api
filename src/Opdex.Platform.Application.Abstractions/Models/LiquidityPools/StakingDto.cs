using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class StakingDto
    {
        public MarketTokenDto Token { get; set; }
        public FixedDecimal Weight { get; set; }
        public decimal Usd { get; set; }
        // Todo: Maybe Daily USD Change too
        // Todo: Consider a new model for grouping a value and % change e.g. stakingWeight: { value: '123', dailyPercentChange: 1.12 }
        public decimal DailyWeightChangePercent { get; set; }
        public bool Nominated { get; set; }
    }
}
