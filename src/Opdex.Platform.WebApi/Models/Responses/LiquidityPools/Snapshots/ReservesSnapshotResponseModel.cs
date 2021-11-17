using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class ReservesSnapshotResponseModel
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
        public decimal Usd { get; set; }
    }
}
