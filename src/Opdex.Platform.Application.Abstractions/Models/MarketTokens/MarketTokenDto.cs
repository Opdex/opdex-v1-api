using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.MarketTokens;

public class MarketTokenDto : TokenDto
{
    public Address Market { get; set; }
    public Address LiquidityPool { get; set; }
}
