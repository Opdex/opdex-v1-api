using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens
{
    /// <summary>
    /// Market token details.
    /// </summary>
    public class MarketTokenResponseModel : TokenResponseModel
    {
        /// <summary>
        /// The market contract address that the token is included in with an associated liquidity pool.
        /// </summary>
        /// <example>t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH</example>
        [NotNull]
        public Address Market { get; set; }

        /// <summary>
        /// The associated liquidity pool contract in the market for the token.
        /// </summary>
        /// <example>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</example>
        [NotNull]
        public Address LiquidityPool { get; set; }
    }
}
