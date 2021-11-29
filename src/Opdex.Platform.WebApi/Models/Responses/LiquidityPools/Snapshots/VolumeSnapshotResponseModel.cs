using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class VolumeSnapshotResponseModel
    {
        /// <summary>
        /// The amount of CRS tokens swapped.
        /// </summary>
        [NotNull]
        public FixedDecimal Crs { get; set; }

        /// <summary>
        /// The amount of SRC tokens swapped.
        /// </summary>
        [NotNull]
        public FixedDecimal Src { get; set; }

        /// <summary>
        /// The volume amount in USD.
        /// </summary>
        [NotNull]
        public decimal Usd { get; set; }
    }
}
