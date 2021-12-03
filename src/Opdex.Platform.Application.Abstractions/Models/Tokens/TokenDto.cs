using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class TokenDto
{
    public ulong Id { get; set; }
    public Address Address { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Decimals { get; set; }
    public ulong Sats { get; set; }
    public FixedDecimal TotalSupply { get; set; }
    public TokenSummaryDto Summary { get; set; }
}