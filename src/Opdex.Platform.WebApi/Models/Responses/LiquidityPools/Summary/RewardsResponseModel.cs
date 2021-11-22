using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary
{
    public class RewardsResponseModel
    {
        /// <summary>
        /// The amount of USD rewards to providers for the current day.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal ProviderDailyUsd { get; set; }

        /// <summary>
        /// The amount of USD rewards to the market, either the market owner of a standard market or stakers of a staking market for the current day.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal MarketDailyUsd { get; set; }

        /// <summary>
        /// The total amount of USD rewards generated from token swaps for the current day.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal TotalDailyUsd { get; set; }
    }
}
