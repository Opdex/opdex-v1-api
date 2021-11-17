using NJsonSchema.Annotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary
{
    public class VolumeResponseModel
    {
        /// <summary>
        /// The daily volume amount in USD.
        /// </summary>
        [NotNull]
        public decimal DailyUsd { get; set; }
    }
}
