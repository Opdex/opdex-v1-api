using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary
{
    public class ReservesResponseModel
    {
        /// <summary>
        /// The total amount of locked CRS tokens.
        /// </summary>
        [NotNull]
        public FixedDecimal Crs { get; set; }

        /// <summary>
        /// The total amount of locked SRC tokens.
        /// </summary>
        [NotNull]
        public FixedDecimal Src { get; set; }

        /// <summary>
        /// The total amount of locked reserves.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal Usd { get; set; }

        /// <summary>
        /// The percentage change of liquidity for the day.
        /// </summary>
        [NotNull]
        [Range(double.MinValue, double.MaxValue)]
        public decimal DailyUsdChangePercent { get; set; }
    }
}
