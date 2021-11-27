using NJsonSchema.Annotations;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools
{
    public class LiquidityPoolTokenGroupResponseModel
    {
        /// <summary>
        /// CRS token details and pricing.
        /// </summary>
        [NotNull]
        public TokenResponseModel Crs { get; set; }

        /// <summary>
        /// The SRC token's details and pricing.
        /// </summary>
        [NotNull]
        public TokenResponseModel Src { get; set; }

        /// <summary>
        /// The liquidity pool token's details and pricing.
        /// </summary>
        [NotNull]
        public TokenResponseModel Lp { get; set; }
    }
}
