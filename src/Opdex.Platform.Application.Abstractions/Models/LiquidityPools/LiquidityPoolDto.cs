using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools;

public class LiquidityPoolDto
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public decimal TransactionFee { get; set; }
    public MarketTokenDto SrcToken { get; set; }
    public MarketTokenDto LpToken { get; set; }
    public TokenDto CrsToken { get; set; }
    public LiquidityPoolSummaryDto Summary { get; set; }
}