using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots
{
    public class StakingSnapshotResponseModel
    {
        /// <summary>
        /// The total number of tokens staking.
        /// </summary>
        [NotNull]
        public OhlcFixedDecimalResponseModel Weight { get; set; }

        /// <summary>
        /// The total USD amount staking.
        /// </summary>
        [NotNull]
        public OhlcDecimalResponseModel Usd { get; set; }
    }
}
