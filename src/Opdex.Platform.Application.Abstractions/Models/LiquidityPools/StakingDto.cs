using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class StakingDto
    {
        public UInt256 Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }

        public void SetDailyChange(UInt256 previousWeight)
        {
            WeightDailyChange = Weight.PercentChange(previousWeight, TokenConstants.Opdex.Decimals);
        }
    }
}
