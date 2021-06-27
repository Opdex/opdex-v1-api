using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class StakingDto
    {
        public string Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }

        public void SetDailyChange(string previousWeight)
        {
            if (!previousWeight.HasValue() || previousWeight.Equals("0") || !previousWeight.IsNumeric())
            {
                WeightDailyChange = 0.00m;
                return;
            }

            const int decimals = TokenConstants.Opdex.Decimals;
            var previousWeightDecimal = previousWeight.ToRoundedDecimal(decimals, decimals);
            var currentWeight = Weight.ToRoundedDecimal(decimals, decimals);
            var weightDailyChange = (currentWeight - previousWeightDecimal) / previousWeightDecimal * 100;

            WeightDailyChange = Math.Round(weightDailyChange, 2, MidpointRounding.AwayFromZero);
        }
    }
}
