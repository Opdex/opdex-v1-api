using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class StakingSnapshotResponseModel
    {
        /// <summary>
        /// The total number of tokens staking.
        /// </summary>
        [NotNull]
        public FixedDecimal Weight { get; set; }

        /// <summary>
        /// The total USD amount staking.
        /// </summary>
        [NotNull]
        public decimal Usd { get; set; }
    }
}
