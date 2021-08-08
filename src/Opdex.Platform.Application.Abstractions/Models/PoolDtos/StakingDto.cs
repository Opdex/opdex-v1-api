using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class StakingDto
    {
        public string Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }

        public void SetDailyChange(string previousWeight)
        {
            WeightDailyChange = Weight.PercentChange(previousWeight, TokenConstants.Opdex.Decimals);
        }
    }
}
