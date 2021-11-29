using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary
{
    public class CostResponseModel
    {
        /// <summary>
        /// The amount of CRS tokens worth 1 full SRC token.
        /// </summary>
        [NotNull]
        public FixedDecimal CrsPerSrc { get; set; }

        /// <summary>
        /// The amount of SRC tokens worth 1 full CRS token.
        /// </summary>
        [NotNull]
        public FixedDecimal SrcPerCrs { get; set; }
    }
}
