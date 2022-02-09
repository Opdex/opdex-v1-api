using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

public class MarketTokenGroupResponseModel
{
    /// <summary>
    /// CRS token details and pricing.
    /// </summary>
    public TokenResponseModel Crs { get; set; }

    /// <summary>
    /// Staking token details and pricing.
    /// </summary>
    public TokenResponseModel Staking { get; set; }
}
