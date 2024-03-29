using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools;

/// <summary>
/// Liquidity pool token pairing details.
/// </summary>
public class LiquidityPoolTokenGroupResponseModel
{
    /// <summary>
    /// CRS token details and pricing.
    /// </summary>
    public TokenResponseModel Crs { get; set; }

    /// <summary>
    /// SRC token details and pricing.
    /// </summary>
    public TokenResponseModel Src { get; set; }

    /// <summary>
    /// Pairing liquidity pool token details and pricing.
    /// </summary>
    public TokenResponseModel Lp { get; set; }

    /// <summary>
    /// Staking token details and pricing.
    /// </summary>
    public TokenResponseModel Staking { get; set; }
}
