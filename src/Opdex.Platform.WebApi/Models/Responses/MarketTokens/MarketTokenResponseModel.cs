using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens;

/// <summary>
/// Market token details.
/// </summary>
public class MarketTokenResponseModel : TokenResponseModel
{
    /// <summary>
    /// The market contract address that the token is included in with an associated liquidity pool.
    /// </summary>
    /// <example>t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb</example>
    public Address Market { get; set; }

    /// <summary>
    /// The associated liquidity pool contract in the market for the token.
    /// </summary>
    /// <example>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</example>
    public Address LiquidityPool { get; set; }
}