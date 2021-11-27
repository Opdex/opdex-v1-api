using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class StakingDto
    {
        public MarketTokenDto Token { get; set; }
        public FixedDecimal Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal DailyWeightChangePercent { get; set; }
        public bool Nominated { get; set; }
    }
}
