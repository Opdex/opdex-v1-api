using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens
{
    public class MarketTokenResponseModel : TokenResponseModel
    {
        /// <summary>
        /// The market contract address that the token is included in with an associated liquidity pool.
        /// </summary>
        [NotNull]
        public Address Market { get; set; }

        /// <summary>
        /// The associated liquidity pool contract in the market for the token.
        /// </summary>
        [NotNull]
        public Address LiquidityPool { get; set; }
    }
}
