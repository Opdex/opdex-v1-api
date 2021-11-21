using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class ReservesSnapshotResponseModel
    {
        /// <summary>
        /// The total amount of locked CRS tokens.
        /// </summary>
        [NotNull]
        public OhlcFixedDecimalResponseModel Crs { get; set; }

        /// <summary>
        /// The total amount of locked SRC tokens.
        /// </summary>
        [NotNull]
        public OhlcFixedDecimalResponseModel Src { get; set; }

        /// <summary>
        /// The total amount of locked reserves.
        /// </summary>
        [NotNull]
        public OhlcDecimalResponseModel Usd { get; set; }
    }
}
